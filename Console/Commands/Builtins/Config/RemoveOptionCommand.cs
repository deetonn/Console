
using Console.UserInterface;

namespace Console.Commands.Builtins.Config;

public class RemoveOptionCommand : BaseBuiltinCommand
{
    public override string Name => "optrm";
    public override string Description => "Remove an option from the configuration";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, Terminal parent)
    {
        base.Run(args, parent);

        if (args.Count < 1)
        {
            return DisplayUsage(parent.Ui);
        }

        if (args.Contains("--help"))
            return DisplayUsage(parent.Ui);

        var option = args.First();
        var removed = parent.Settings.RemoveOption(option);

        if (!removed)
        {
            parent.Ui.DisplayLinePure($"No existing command to remove named '{option}'");
            return -1;
        }

        parent.Ui.DisplayLinePure($"Successfully removed");

        return 0;
    }

    private int DisplayUsage(IUserInterface terminal)
    {
        terminal.DisplayLinePure($"{Name} -- USAGE\n");

        terminal.DisplayLinePure($"{Name} <option-name>");
        terminal.DisplayLinePure("\nOption Name: The name of the option to remove.");

        return -1;
    }
}
