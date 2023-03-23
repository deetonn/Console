
using System.Security;

namespace Console.Commands.Builtins.Config;

public class ViewOptionsCommand : BaseBuiltinCommand
{
    public override string Name => "optview";
    public override string Description => "View the configuration settings";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, Terminal parent)
    {
        base.Run(args, parent);

        foreach (var option in parent.Settings.Options)
        {
            parent.Ui.DisplayLine($"[{option.TechnicalName}]: {option.VisualName} (Value: {option.Value})");
        }

        return 0;
    }
}
