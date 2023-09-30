using Console.Errors;
using Pastel;
using System.Drawing;

namespace Console.Commands.Builtins;

public class HelpCommand : BaseBuiltinCommand
{
    public override string Name => "help";
    public override string Description => "List all active commands.";
    public override DateTime? LastRunTime { get; set; } = null;
    public override CommandResult Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);
        var wantsAll = args.Contains("--all");
        var wantsHelp = args.Contains("--help");

        if (wantsHelp)
            return DisplayUsage();

        // handle syntax of --contains=TEXT
        if (args.Any(x => x.StartsWith("--contains=")))
        {
            var text = args.First(x => x.StartsWith("--contains=")).Split('=')[1];
            var commands = parent.Commands.Elements.Where(x => x.Name.Contains(text));
            foreach (var command in commands)
            {
                DisplayCommand(command);
            }

            return CommandReturnValues.DontShowText;
        }

        if (wantsAll)
        {
            foreach (var command in parent.Commands.Elements)
            {
                if (command is not null)
                    DisplayCommand(command);
            }
        }
        else
        {
            var builtins = parent.Commands.Elements.Where(x => x is BaseBuiltinCommand);
            foreach (var command in builtins)
            {
                DisplayCommand(command);
            }

            return CommandReturnValues.DontShowText;
        }

        return CommandReturnValues.DontShowText;
    }

    public static string NameOutput(string name) => name.Pastel(Color.SkyBlue);

    int DisplayUsage()
    {
        WriteLine($"{Name} - usage");
        WriteLine("  --all: display all commands, including ones loaded from PATH.");
        return 0;
    }

    void DisplayCommand(ICommand command)
    {
        try
        {
            WriteLine($"[blue]{command.Name}[/]: [white]{command.Description}[/]");
        }
        catch
        {
            WriteLine("[italic][red]failed to display command[/][/]");
        }
    }

    public override string DocString => $@"
This command will display a list of all active commands.

Usage:
    {Name} [[--all]] [[--contains=<value>]]

Options:
  --all: If this flag is present, all commands will be displayed, including ones loaded from PATH.
  --contains=<value>: If this is present, the command will only output commands whos name include
                      <value>. This check is case sensitive.
";
}