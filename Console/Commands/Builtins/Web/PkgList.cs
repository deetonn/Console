using Console.Errors;

namespace Console.Commands.Builtins.Web;

public class PkgList : BaseBuiltinCommand
{
    public override string Name => "pkg-list";
    public override string Description => "List all packages available to install";
    public override DateTime? LastRunTime { get; set; } = null;
    public override CommandResult Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        foreach (var (key, value) in PkgInstall.PackageDirectory)
        {
            WriteLine($"[cyan italic]{key}[/] -- (from [blue]{value.DownloadLink}[/])\n{value.Description}");
        }

        return CommandReturnValues.DontShowText;
    }

    public override string DocString => $@"
This command will list all available packages to install.

It will also show the description of the package, and where it is downloaded from.

The packages are displayed like this:
  Name -- DownloadLink

Use the pkg-install command to install a package from this list.
Feel free to check the download links before installing a package.
";
}
