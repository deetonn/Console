using Console.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console.Commands.Builtins.DirBased;

public class MkDirCommand : BaseBuiltinCommand
{
    public override string Name => "mkdir";

    public override string Description => "Creates a new directory.";

    public override string DocString => @$"
Create a new directory in the current directory, or, if the path is rooted, a specified directory.

Usage:
    {Name} <path>
";

    public override CommandResult Run(List<string> args, IConsole parent)
    {
        if (args.Count < 1)
        {
            return Error()
                .WithMessage("Invalid command line arguments.")
                .WithNote("mkdir: missing operand")
                .WithNote("expected a `path`.")
                .Build();
        }

        var path = args[0];

        if (!Path.IsPathRooted(path))
        {
            path = Path.Combine(parent.WorkingDirectory, path);
        }

        try
        {
            Directory.CreateDirectory(path);
        }
        catch (Exception ex)
        {
            return Error()
                .WithMessage("failed to create directory.")
                .WithNote($"message: {ex.Message}")
                .Build();
        }

        return 0;
    }
}
