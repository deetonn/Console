
namespace Console.Commands.Builtins.DirBased;

public class TouchCommand : BaseBuiltinCommand
{
    public override string Name => "touch";
    public override string Description => "Creates a file with the specified name.";

    public override int Run(List<string> args, Terminal target)
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
            target.WriteLine("touch: missing file operand");
            target.WriteLine("Try 'touch --help' for more information.");
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

        File.Create(path).Dispose();
        return CommandReturnValues.SafeExit;
    }
}
