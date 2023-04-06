
using Console.UserInterface;

namespace Console.Commands.Builtins.Config;

public class EditOptionCommand : BaseBuiltinCommand
{
    public override string Name => "optedit";
    public override string Description => "Edit the configuration";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, Terminal parent)
    {
        base.Run(args, parent);

        if (args.Count < 2)
        {
            return DisplayUsage(parent.Ui);
        }

        // We save on the desktop.
        parent.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        var option = args[0];
        var value = args[1];
        var create = args.Contains("--create");
        
        if (!parent.Settings.OptionExists(option))
        {
            if (!create)
            {
                parent.Ui.DisplayLine($"No such command to edit. `{option}`");
                return -2;
            }

            parent.Settings.SetOption(option, (opt) =>
            {
                opt.Value = value;
                return opt;
            });

            parent.Ui.DisplayLine($"Created option `{option}` with a value of `{value}`");

            return 0;
        }

        parent.Settings.SetOption(option, (opt) =>
        {
            opt.Value = value;
            return opt;
        });

        parent.Ui.DisplayLine($"Edited option `{option}`, new value is `{value}`");

        return 0;
    }

    private int DisplayUsage(IUserInterface terminal)
    {
        terminal.DisplayLinePure($"{Name} -- USAGE\n");

        terminal.DisplayLinePure($"{Name} <option-name> <value> [--create]");
        terminal.DisplayLinePure("\nOption Name: The name of the option to edit or create.");
        terminal.DisplayLinePure("Value: The value to assign to said setting.");
        terminal.DisplayLinePure("--create: If present, this will create a new key.");

        return -1;
    }
}
