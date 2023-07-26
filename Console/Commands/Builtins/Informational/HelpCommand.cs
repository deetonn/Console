using Pastel;
using System.Drawing;

namespace Console.Commands.Builtins;

public class HelpCommand : BaseBuiltinCommand
{
    public override string Name => "help";
    public override string Description => "List all active commands.";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, Terminal parent)
    {
        base.Run(args, parent);
        var wantsAll = args.Contains("--all");
        var wantsHelp = args.Contains("--help");

        if (wantsHelp)
            return DisplayUsage();

        if (wantsAll)
        {
            foreach (var command in parent.Commands.Elements)
            {
                parent.Ui.DisplayLine($"{NameOutput(command.Name)}: {command.Description}");
            }
        }
        else
        {
            var builtins = parent.Commands.Elements.Where(x => x is BaseBuiltinCommand);
            foreach (var command in builtins)
            {
                parent.Ui.DisplayLine($"{NameOutput(command.Name)}: {command.Description}");
            }

            return 2;
        }

        return 0;
    }

    public static string NameOutput(string name) => name.Pastel(Color.SkyBlue);

    int DisplayUsage()
    {
        WriteLine($"{Name} - usage");
        WriteLine("  --all: display all commands, including ones loaded from PATH.");
        return 0;
    }
}