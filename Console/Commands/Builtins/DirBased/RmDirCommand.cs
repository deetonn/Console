
using Console.Errors;

namespace Console.Commands.Builtins.DirBased;

public class RmDirCommand : BaseBuiltinCommand
{
    public override string Name => "rmdir";
    public override string Description => "Remove a directory";
    public override DateTime? LastRunTime { get; set; } = null;
    public override CommandResult Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        if (args.Count < 1)
        {
            return DisplayUsage();
        }

        if (args.Contains("--help"))
            return DisplayUsage();

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

    private CommandError DisplayUsage()
    {
        return Error()
            .WithMessage("invalid usage")
            .WithNote($"usage: {Name} <path> [[...options]]")
            .WithNote($"path: the path to remove.")
            .WithNote($"use \"docs {Name}\" for more information.")
            .Build();
    }

    public override string DocString => $@"
This command will attempt to remove a directory.

If the argument supplied is relative, the directory removed will be relative to the CWD.

The syntax is as follows:
    {Name} <path> [[...options]]
       --all: remove the directory without any warnings, will remove all files/directorys within it too.
";
}
