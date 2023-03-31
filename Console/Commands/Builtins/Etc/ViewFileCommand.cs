
using Console.UserInterface;
using System.IO;

namespace Console.Commands.Builtins.Etc;

public class ViewFileCommand : BaseBuiltinCommand
{
    public override string Name => "vwf";
    public override string Description => "View a file contents within the terminal";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, Terminal parent)
    {
        base.Run(args, parent);

        if (args.Count < 1)
        {
            return DisplayUsage();
        }

        if (Path.IsPathRooted(args[0]))
        {
            var path0 = Path.GetFullPath(args[0]);
            if (!File.Exists(path0))
            {
                WriteLine($"No such file '{path0}'");
                return -1;
            }
            var contents0 = File.ReadAllText(path0);
            return DisplayFileContents(contents0);
        }

        var path = Path.Combine(parent.WorkingDirectory, args[0]);
        if (!File.Exists(path))
        {
            WriteLine($"No such file '{path}'");
            return -1;
        }

        var contents = File.ReadAllText(path);
        return DisplayFileContents(contents);
    }

    private int DisplayUsage()
    {
        WriteLine($"USAGE -- {Name}");
        WriteLine($"{Name} <file-name>");

        return -1;
    }

    private int DisplayFileContents(string contents)
    {
        var lines = contents.Split('\n');

        for (int i = 0; i < lines.Length; ++i)
        {
            var line = lines[i];
            var lno = i + 1;
            WriteLine($"{lno} | {line}");
        }

        return 0;
    }
}
