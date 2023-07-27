
using Pastel;
using System.Drawing;
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
            WriteLine($"[{option.TechnicalName}]: {option.VisualName} (Value: {option.Value})");
        }

        var savePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        WriteLine($"\nAll options are saved inside the path ` {savePath} `");

        return 0;
    }
}
