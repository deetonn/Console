using Console.Commands.Builtins.Etc;
using Console.Errors;
using Console.Utilitys;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using SystemConsole = global::System.Console;

namespace Console.Commands;

public class BaseCommandCentre : ICommandCentre
{
    private const string PathVariableName = "PATH";
    private Thread _loadingThread;


    public BaseCommandCentre(IConsole parent)
    {
        Elements = LoadBuiltinCommands();
        LoadPathExecutables(parent);
        PausedCommands = new List<ICommand>();
    }

    public IList<ICommand> Elements { get; }
    public IList<ICommand> PausedCommands { get; }

    public CommandResult Run(string name, List<string> args, IConsole owner)
    {
        if (_loadingThread.IsAlive)
        {
            return new CommandErrorBuilder()
                .WithSource("<global>")
                .WithMessage("commands have not yet loaded, please wait a moment.")
                .WithNote("it can take up to 5 seconds for commands to fully load.")
                .Build();
        }

        var command = Elements
                   .FirstOrDefault(x => x.Name.ToLower().Equals(name.ToLower()));

        if (command == null)
        {
            return new CommandErrorBuilder()
                .WithSource(owner.GetLastExecutedString())
                .WithMessage($"the command \"{name}\" does not exist.")
                .WithNote("cases are not sensitive, this command was not found.")
                .WithNote("use \"help\" for builtin commands, and \"help --all\" for all commands.")
                .Build();
        }

        var result = command.Run(args, owner);
        owner.EventHandler.HandleOnCommandExecuted(new(command));
        return result;
    }

    public bool CommandExists(string name)
    {
        return Elements.Any(x => x.Name == name);
    }

    public bool CommandExists(string name, [NotNullWhen(true)] out ICommand? command)
    {
        command = Elements.FirstOrDefault(x =>
        {
            if (x.Name == name)
            {
                return true;
            }
            return false;
        });
        return command != null;
    }

    public List<ICommand> LoadBuiltinCommands()
    {
        var instances = new List<ICommand>();
        var types = Assembly.GetExecutingAssembly().GetTypes();
        var @base = typeof(ICommand);

        var index = 0;
        for (; index < types.Length; index++)
        {
            var type = types[index];
            if (!type.IsAssignableTo(@base)
                || type == @base
                || type == typeof(BaseBuiltinCommand)
                || type == typeof(PathFileCommand)
                || type == typeof(AsyncCommand)
                || type == typeof(AliasBuiltinCommand))
                continue;
            var instance = Activator.CreateInstance(type);
            if (instance is null)
                continue;
            instances.Add((ICommand)instance);
        }

        Logger().LogInfo(this, $"Loading {instances.Count} builtin commands");
        return instances;
    }

    private static string GetSearchPatternForOs()
    {
        // If the OS is windows, use "*.exe"
        // Otherwise, use "*"
        return Environment.OSVersion.Platform == PlatformID.Win32NT ? "*.exe" : "*";
    }

    public static char PathSep
    {
        get
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                return ';';
            }
            return ':';
        }
    }

    public void LoadPathExecutables(IConsole terminal)
    {
        var logger = Singleton<ILogger>.Instance();

        var path = Environment.GetEnvironmentVariable(PathVariableName);
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        var dirs = path.Split(PathSep);

        _loadingThread = new Thread(() =>
        {
#if DEBUG
            SystemConsole.WriteLine("path command thread launched!");
#endif

            var results = new List<ICommand>();

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                results = LoadFromWindowsPath(dirs);
            }
            else
            {
                results = LoadFromGnuBasedPath(dirs);
            }

            ((List<ICommand>)Elements).AddRange(results);

#if DEBUG
            SystemConsole.WriteLine("path command thread finished!");
#endif

            terminal.Ui.SetTitle($"Success | Loaded {Elements.Count} commands from disk.");
        });

        _loadingThread.Start();
    }

    private List<ICommand> LoadFromWindowsPath(string[] dirs)
    {
        var results = new List<ICommand>();

        foreach (var directory in dirs)
        {
            if (string.IsNullOrEmpty(directory))
            {
                continue;
            }

            try
            {
                var commands = Directory.EnumerateFiles(directory, GetSearchPatternForOs());
                Parallel.ForEach(commands, fileInfo =>
                {
                    var info = new FileInfo(fileInfo);
                    results.Add(new PathFileCommand(info));
                });

            }
            catch (Exception ex) when (ex is DirectoryNotFoundException || ex is IOException)
            {
                Logger().LogError(this, $"Failed to load path `{directory}` [{ex.Message}]");
            }
            catch (ArgumentException ex)
            {
                Logger().LogError(this, $"Invalid path entry `{directory}` [{ex.Message}]");
            }
        }

        return results;
    }
    
    private List<ICommand> LoadFromGnuBasedPath(string[] dirs)
    {
        Logger().LogInfo(this, $"loading {dirs.Length} GNU based paths.");
        var results = new List<ICommand>();

        foreach (var dir in dirs)
        {
            // each directory will only have the binarys in them, so just iterate
            // them all and add them to the list
            IEnumerable<string>? files;
            try
            {
                files = Directory.EnumerateFiles(dir);
            }
            catch (Exception e)
            {
                Logger().LogWarning(this, $"failed to load PATH variable. {e.Message}");
                return Array.Empty<ICommand>().ToList();
            }
            foreach (var file in files)
            {
                FileInfo info;
                try
                {
                    info = new FileInfo(file);
                }
                catch (Exception e)
                {
                    Logger().LogWarning(this, $"the file `{file}` in a path variable directory does not exist. ({e.Message})");
                    continue;
                }
                if (info.Extension != string.Empty)
                {
                    continue;
                }
                results.Add(new PathFileCommand(info));
            }
        }

        return results;
    }

    public CommandResult ExecuteFrom(IConsole parent, string name, params string[] args)
    {
        if (!CommandExists(name, out var command))
        {
            return CommandReturnValues.NoSuchCommand;
        }

        if (_loadingThread.IsAlive)
        {
            return new CommandErrorBuilder()
                .WithSource("<global>")
                .WithMessage("commands have not yet loaded, please wait a moment.")
                .WithNote("it can take up to 5 seconds for commands to fully load.")
                .Build();
        }

        var result = command.Run(args.ToList(), parent);
        parent.EventHandler.HandleOnCommandExecuted(new(command));
        return result;
    }

    public void LoadCustomCommand(ICommand command)
    {
        Elements.Add(command);
    }

    public ICommand? GetCommand(string name)
    {
        return Elements
            .Where(x => x.Name == name)
            .FirstOrDefault();
    }
}