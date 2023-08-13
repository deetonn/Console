
namespace Console.Commands.Builtins.DirBased;

public class TouchCommand : BaseBuiltinCommand
{
    public override string Name => "touch";
    public override string Description => "Creates a file with the specified name.";

    public override int Run(List<string> args, IConsole target)
    {
        base.Run(args, target);

        if (args.Count == 0 || args.Contains("--help"))
        {
            WriteLine($"{Name} - usage\n");
            WriteLine($"  {Name} <file-name>");
            return 0;
        }

        if (string.IsNullOrWhiteSpace(args[0]))
        {
            WriteLine("missing file operand");
            WriteLine("try 'touch --help' for more information.");
            return CommandReturnValues.BadArguments;
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

    public override string DocString => $@"
This command will create a file or update an existing files last read date.

Example:
    touch file.txt
    touch new_file.c

NOTE: This command will not create directories.
";
}
