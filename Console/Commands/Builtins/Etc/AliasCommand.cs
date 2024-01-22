using CommandLine;
using Console.Commands.Builtins.Arguments;
using Console.Errors;
using Newtonsoft.Json;

namespace Console.Commands.Builtins.Etc;

public record class Alias
    (string Name, List<string> Commands, string Description);

public class AliasBuiltinCommand : BaseBuiltinCommand
{
    public Alias Alias { get; }

    public override string Name { get; }
    public override string Description => Alias.Description;

    public AliasBuiltinCommand(Alias alias)
    {
        Name = alias.Name;
        Alias = alias;
    }

    public override CommandResult Run(List<string> args, IConsole parent)
    {
        var toExecute = Alias.Commands;
        CommandResult lastResult = 0;

        foreach (var line in toExecute)
        {
            var split = line.Split(' ');
            if (split.Length == 0)
                return 0;
            var name = split.First();
            var line_args = split.Skip(1);
            if (parent is Terminal terminal)
            {
                line_args = terminal.HandleInlineEnvironmentVariables(string.Empty, line_args.ToArray()).Item1!;
            }
            lastResult = parent.Commands.ExecuteFrom(parent, name, line_args.ToArray());
        }

        return lastResult;
    }
}

public class AliasCommand : BaseBuiltinCommand
{
    public const string AliasConfigFileName = "aliases.json";

    public List<Alias> Aliases { get; set; } = new();

    public override string Name => "alias";
    public override string Description => "Alias a string to a command.";

    // TODO: when the user enters any environment variable into the input
    // string, do not replace it at that time. It should be replaced when 
    // the alias is executed. This will probably need something done inside
    // of the actual terminal handler before a command is executed.

    public override CommandResult Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        if (args.Count < 1)
        {
            return Error()
                .WithMessage("Invalid command line arguments.")
                .WithNote("alias: missing operand")
                .WithNote("expected a `name`.")
                .Build();
        }

        if (args.Contains("--clear"))
        {
            // example: alias --clear
            // first ask the user if they are sure they want to clear all aliases
            // then clear all aliases
            // then save the aliases
            // then return 0
            var result = ReadLine("Are you sure you want to clear all aliases? [[y/N]] ");

            if (result?.ToLower() != "y")
            {
                return 0;
            }

            var count = Aliases.Count;

            Aliases.Clear();
            Save(parent);
            WriteLine($"Cleared all aliases (total of {count})");

            // remove all elements in the parent that are aliases.
            (parent.Commands.Elements as List<ICommand>)!.RemoveAll(x => x is AliasBuiltinCommand);

            return 0;
        }

        if (args.Contains("--remove"))
        {
            // when "--remove" is specified, remove the alias after the token "--remove"
            // example: alias --remove test
            var index = args.IndexOf("--remove");
            if (index == -1)
            {
                return Error()
                    .WithMessage("Invalid command line arguments.")
                    .WithNote("alias: missing operand")
                    .WithNote("expected a `name`.")
                    .Build();
            }
            var identToRemove = args[index + 1];
            var aliasToRemove = Aliases.FirstOrDefault(x => x.Name == identToRemove);
            if (aliasToRemove == null)
            {
                return Error()
                    .WithMessage("Invalid command line arguments.")
                    .WithNote("alias: invalid argument")
                    .WithNote($"no alias with name `{identToRemove}` exists.")
                    .Build();
            }

            Aliases.Remove(aliasToRemove);
            Save(parent);
            return 0;
        }

        // the syntax for alias is like this:
        // alias identifier="script & other arg1"
        var allArgs = string.Join(' ', args);

        var split = allArgs.Split('=');
        if (split.Length != 2)
        {
            return Error()
                .WithMessage("Invalid command line arguments.")
                .WithNote("alias: invalid argument")
                .WithNote("expected a `name` and a `command` separated by `=`.")
                .WithNote("example: alias helpa=\"help --all\"")
                .Build();
        }
        var name = split[0];
        var command = split[1].Replace("\"", string.Empty);

        var alias = new Alias(name, [command], $"Alias for `{command}`.");
        Aliases.Add(alias);
        Save(parent);
        
        // register the command in the parent
        parent.Commands.LoadCustomCommand(new AliasBuiltinCommand(alias));

        WriteLine("Alias created successfully.");

        return 0;
    }

    public override void OnInit(IConsole parent)
    {
        var path = Path.Join(parent.GetConfigPath(), "aliases.json");
        Logger().LogDebug(this, $"Alias configuration is being loaded from `{path}`");

        if (!File.Exists(path))
        {
            File.Create(path).Close();
        }
        else
        {
            Logger().LogDebug(this, $"Loading alias config from path `{path}`");

            var fileContents = File.ReadAllText(path);
            Aliases = JsonConvert.DeserializeObject<List<Alias>>(fileContents)!;
            Aliases ??= [];
            Logger().LogInfo(this, $"Loaded {Aliases.Count} saved aliases.");

            foreach (var commandAlias in Aliases)
            {
                parent.Commands.LoadCustomCommand(new AliasBuiltinCommand(commandAlias));
            }
        }

        if (!parent.Commands.Elements.Any(x => x.Name == "ls"))
        {
            // load "ls" as an alias
            parent.Commands.LoadCustomCommand(
                new AliasBuiltinCommand(
                    new Alias("dir", [], "list files and folders in the current directory.")
                )
            );
        }
    }

    private void Save(IConsole parent)
    {
        var path = Path.Join(parent.GetConfigPath(), AliasConfigFileName);
        if (!File.Exists(path))
        {
            try
            {
                File.Create(path).Close();
            }
            catch (Exception e)
            {
                Logger().LogError(this, $"Failed to save aliases. [{e.Message}]");
                return;
            }
        }

        var json = JsonConvert.SerializeObject(Aliases, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(path, json);
    }

    public override string DocString => $@"
This command will alias a string to a command.
These aliases are saved between sessions, and are loaded on startup.

The syntax for this command is as follows:
  {Name} alias_name=""command1 arg1 & command2 arg2""

  The string containing the command can contain `&` to split commands.
  The above example would execute sequentially, and the result of the last command would be returned.

  This alias: {Name} test=""help --all & echo hello""
     Would execute the following commands:
        help --all
        echo hello

This is unix terminal style syntax. The alias name is the first argument, and the command is the second.
";
}
