
using Console.Errors;

namespace Console.Commands.Builtins.DirBased;

public class TouchCommand : BaseBuiltinCommand
{
    public override string Name => "touch";
    public override string Description => "Creates a file with the specified name.";

    public override CommandResult Run(List<string> args, IConsole target)
    {
        base.Run(args, target);

        if (args.Count == 0 || args.Contains("--help"))
        {
            return Usage();
        }

        if (string.IsNullOrWhiteSpace(args[0]))
        {
            return Error()
                .WithMessage("missing file operand")
                .WithNote($"try \"{Name} --help\" for more information.")
                .Build();
        }

        var path = Path.GetFullPath(args[0]);
        if (File.Exists(path))
        {
            // If the file exists, change the last read date
            // to the current date.
            File.SetLastWriteTimeUtc(path, DateTime.UtcNow);
            return 0;
        }

        using var _ = File.Create(path);
        return 0;
    }

    private CommandError Usage()
    {
        return Error()
            .WithMessage("invalid usage")
            .WithNote($"usage: {Name} <file-name>")
            .WithNote("file-name: the name of the file to touch, or the name of the file to create.")
            .Build();
    }

    public override string DocString => $@"
This command will create a file or update an existing files last read date.

Example:
    touch file.txt
    touch new_file.c

NOTE: This command will not create directories.
";
}
