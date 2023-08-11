using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public override int Run(List<string> args, IConsole parent)
    {
        var toExecute = Alias.Commands;
        int lastResult = 0;

        foreach (var line in toExecute)
        {
            var split = line.Split(' ');
            if (split.Length == 0)
                return 0;
            var name = split.First();
            var line_args = split.Skip(1);
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

    public override int Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        if (args.Contains("--remove"))
        {
            // expect args[0] to be remove.
            if (args.Count != 2)
            {
                WriteLine("--remove: usage: alias --remove <name>");
                return -1;
            }

            var nameToRemove = args[1];
            var aliasToRemove = Aliases.FirstOrDefault(x => x.Name == nameToRemove);

            if (aliasToRemove is null)
            {
                WriteLine($"No alias with name `{nameToRemove}` exists.");
                return -1;
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
            WriteLine("Invalid syntax: usage: alias=\"commands\"");
            return -1;
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
            Logger().LogInfo(this, $"Loaded {Aliases.Count} saved aliases.");

            foreach (var commandAlias in Aliases)
            {
                parent.Commands.LoadCustomCommand(new AliasBuiltinCommand(commandAlias));
            }
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
