using Console.Errors;
using Newtonsoft.Json;

namespace Console.Commands.Builtins.Etc;

public record class Alias
    (string Name, List<string> Commands);

public class AliasBuiltinCommand : BaseBuiltinCommand
{
    public Alias Alias { get; }

    public override string Name { get; }
    public override string Description => "This is an alias command.";

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
                .WithMessage("expected at least one argument")
                .WithNote("usage: [green]alias[/] [italic blue]name[/]=[yellow]\"script...\"[/]")
                .Build();
        }

        var firstArgument = args[0];

        if (firstArgument == "--list")
        {
            foreach (var individual in Aliases)
            {
                WriteLine($"{individual.Name}: \"{string.Join('&', individual.Commands)}\"");
            }

            return 0;
        }

        if (firstArgument == "--remove")
        {
            // expect args[0] to be remove.
            if (args.Count != 2)
            {
                return Error()
                    .WithMessage("--remove: expects a name argument.")
                    .WithNote($"usage: {Name} --remove <name>")
                    .Build();
            }

            var nameToRemove = args[1];
            var aliasToRemove = Aliases.FirstOrDefault(x => x.Name == nameToRemove);

            if (aliasToRemove is null)
            {
                return Error()
                    .WithMessage($"No alias with name `{nameToRemove}` exists.")
                    .Build();
            }

            Aliases.Remove(aliasToRemove);
            Save(parent);

            WriteLine("The alias has been removed. You need to restart for changes to take effect.");

            return 0;
        }

        // The syntax will look like this:
        // alias alias_name="command1 arg1 & command2 arg2"

        var entireCommand = string.Join(' ', args);
        var splitCommand = entireCommand.Split('=');

        if (splitCommand.Length != 2)
        {
            return Error()
                .WithMessage("invalid syntax.")
                .WithNote("usage: [green]alias[/] [italic blue]name[/]=[yellow]\"script...\"[/]")
                .Build();
        }

        var name = splitCommand.First();
        var commands = splitCommand.Last().Split('&').Select(x => x.Trim().Replace("\"", "")).ToList();

        Logger().LogDebug(this, $"Saved alias `{name}`, commands=`{string.Join(", ", commands)}`");

        var alias = new Alias(name, commands);
        Aliases.Add(alias);
        parent.Commands.LoadCustomCommand(new AliasBuiltinCommand(alias));
        Save(parent);

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
            Aliases ??= new List<Alias>();
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
                    new Alias("dir", Array.Empty<string>().ToList()
                    )
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

        var json = JsonConvert.SerializeObject(Aliases, Formatting.Indented);
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
