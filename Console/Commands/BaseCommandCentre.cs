using System.IO;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using Console.Utilitys;

namespace Console.Commands;

public class BaseCommandCentre : ICommandCentre
{
    private const string PathVariableName = "PATH";
    
    public BaseCommandCentre()
    {
        Elements = LoadBuiltinCommands();
        ((List<ICommand>)Elements).AddRange(LoadPathExecutables());
        PausedCommands = new List<ICommand>();
    }
    
    public IList<ICommand> Elements { get; }
    public IList<ICommand> PausedCommands { get; }

    public int Run(string name, List<string> args, Terminal owner)
    {
        return Elements
                   .FirstOrDefault(x => x.Name.ToLower().Equals(name.ToLower()))?
                   .Run(args, owner)
               ?? int.MinValue;
    }

    public bool CommandExists(string name)
    {
        return Elements.Any(x => x.Name == name);
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
                || type == typeof(AsyncCommand))
                continue;
            var instance = Activator.CreateInstance(type);
            if (instance is null)
                continue;
            instances.Add((ICommand)instance);
        }

        return instances;
    }

    public List<ICommand> LoadPathExecutables()
    {
        var logger = Singleton<ILogger>.Instance();

        var path = Environment.GetEnvironmentVariable(PathVariableName);
        if (path is null)
        {
            return Array.Empty<ICommand>().ToList();
        }

        var results = new List<ICommand>();
        var dirs = path.Split(';');

        Task.Run(() =>
        {
            foreach (var directory in dirs)
            {
                // get the files in that directory,
                // construct a PathFileCommand with the FileInfo
                string[]? files;

                try
                {
                    files = Directory.GetFiles(directory, "*.exe");
                }
                catch (DirectoryNotFoundException ex)
                {
                    // The PATH directory does not exist.
                    logger.Err(this, $"failed to load path `{directory}` [{ex.Message}]");
                    continue;
                }
                catch (IOException ex)
                {
                    // The PATH directory points to a file
                    logger.Err(this, $"failed to load path `{directory}` [{ex.Message}]");
                    continue;
                }
                catch (ArgumentException ex)
                {
                    logger.Err(this, $"Invalid path entry `{directory}` [{ex.Message}]");
                    continue;
                }

                var commands = files
                    .Select(x => new FileInfo(x));

                results.AddRange(
                    commands
                        .Select(fileInfo => new PathFileCommand(fileInfo)
                        )
                    );
            }
        });

        return results;
    }

    public int AttemptToQueueCommand(string name, List<string> args, Terminal owner)
    {
        if (!CommandExists(name))
        {
            return CommandReturnValues.NoSuchCommand;
        }

        var instance = Elements.First(x => x.Name == name);

        if (instance is not PathFileCommand command)
        {
            var wrapper = new AsyncCommand(instance);
            PausedCommands.Add(wrapper);
            return 0;
        }
        
        var started = command.StartThenPause(args);
        PausedCommands.Add(instance);
        
        return started ? 0 : CommandReturnValues.FailedToStartProcess;
    }

    public ICommand? FinishQueuedCommand(string command)
    {
        if (PausedCommands.Count == 1)
        {
            return PausedCommands[0];
        }

        return PausedCommands
                .FirstOrDefault(x => x.Name == command);
    }
}