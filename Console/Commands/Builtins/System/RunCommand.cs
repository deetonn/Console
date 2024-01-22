
using Console.Errors;

namespace Console.Commands.Builtins.System;

public class RunCommand : BaseBuiltinCommand
{
    public override string Name => "run";
    public override string Description => "Execute a command from a full path.";
    public override CommandResult Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        if (args.Count < 1)
        {
            return Error()
                .WithMessage("expected at least one argument.")
                .WithNote($"use \"docs {Name}\" for more information.")
                .Build();
        }

        var fullPath = args[0];
        var arguments = args.Skip(1).ToArray();

        try
        {
            // FIXME: this is a hack, but it works for now.
            var process = new PathFileCommand(new FileInfo(fullPath));
            return process.Run([.. arguments], parent);
        }
        catch (Exception e)
        {
            return Error()
                .WithMessage("failed to execute command")
                .WithNote($"{e.Message}")
                .WithNote($"This is likely because the path `{fullPath}` does not exist.")
                .Build();
        }
    }

    public override string DocString => $@"
{Name} -- {Description}

This command will execute an executable file with specified arguments.
This command will not attempt to open files using different applications, just
executable files. (so ending with .exe (windows), or no extension (gnu))

Example:
    {Name} /bin/ls -l
    {Name} C:\Users\user\Documents\myapp.exe
";
}
