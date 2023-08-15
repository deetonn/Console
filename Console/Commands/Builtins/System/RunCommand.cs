
using System.Diagnostics;

namespace Console.Commands.Builtins.System;

public class RunCommand : BaseBuiltinCommand
{
    public override string Name => "run";
    public override string Description => "Execute a command from a full path.";
    public override int Run(List<string> args, IConsole parent)
    {
        if (args.Count < 1)
        {
            WriteLine("expected at least one argument.");
            return -1;
        }

        var fullPath = args[0];
        var arguments = args.Skip(1).ToArray();

        try
        {
            // FIXME: this is a hack, but it works for now.
            var process = new PathFileCommand(new FileInfo(fullPath));
            return process.Run(arguments.ToList(), parent);
        }
        catch (Exception e)
        {
            WriteLine($"error: {e.Message}");
        }

        return CommandReturnValues.DontShowText;
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
