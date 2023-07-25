using Pastel;
using System.Drawing;

namespace Console.Commands.Builtins.Web;

public class PkgList : BaseBuiltinCommand
{
    public override string Name => "pkg-list";
    public override string Description => "List all packages available to install";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, Terminal parent)
    {
        base.Run(args, parent);

        foreach (var (key, value) in PkgInstall.PackageDirectory)
        {
            WriteLine($"{key.Pastel(Color.Cyan)} -- (from {value.DownloadLink.Pastel(Color.MediumBlue)})\n{value.Description}");
        }

        return CommandReturnValues.DontShowText;
    }
}
