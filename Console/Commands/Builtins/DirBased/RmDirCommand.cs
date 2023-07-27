
using Console.UserInterface;

namespace Console.Commands.Builtins.DirBased;

public class RmDirCommand : BaseBuiltinCommand
{
    public override string Name => "rmdir";
    public override string Description => "Remove a directory";
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

        var path = args[0];

        if (!Directory.Exists(path))
        {
            path = Path.Combine(parent.WorkingDirectory, path);
            if (!Directory.Exists(path))
            {
                parent.Ui.DisplayLinePure($"No such file or directory [{path}]");
            }
        }

        var skipChecks = args.Contains("--all");

        if (skipChecks)
        {
            try
            {
                Directory.Delete(path, true);
            }
            catch (Exception ex)
            {
                parent.Ui.DisplayLinePure($"Failed to delete the directory. [{ex.GetType().Name}, {ex.Message}]");
            }
        }
        else
        {
            try
            {
                Directory.Delete(path);
            }
            catch (Exception ex)
            {
                parent.Ui.DisplayLinePure($"Failed to delete the directory. [{ex.GetType().Name}, {ex.Message}]");
            }
        }

        return 0;
    }

    private int DisplayUsage(IUserInterface ui)
    {
        ui.DisplayLinePure($"{Name} -- USAGE\n");

        ui.DisplayLinePure($"rmdir <Path> [...options]\n");
        ui.DisplayLinePure("Path: The path to remove");
        ui.DisplayLinePure("Options:");
        ui.DisplayLinePure("  --all: remove the file without any warnings.");

        return -1;
    }

    public override string DocString => $@"
This command will attempt to remove a directory.

If the argument supplied is relative, the directory removed will be relative to the CWD.

The syntax is as follows:
    {Name} <path> [...options]
       --all: remove the directory without any warnings, will remove all files/directorys within it too.
";
}
