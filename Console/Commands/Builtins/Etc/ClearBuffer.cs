
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Console.Errors;
using Console.Extensions;

namespace Console.Commands.Builtins.Etc;

public class ClearBufferCommand : BaseBuiltinCommand
{
    public override string Name => "clear";
    public override string Description => "Clear the Terminal buffer";
    public override DateTime? LastRunTime { get; set; } = null;
    public override CommandResult Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        parent.Ui.Clear();

        return 1;
    }

    public override string DocString => $@"
This command will clear the Terminal buffer.
";
}

public class ElevateCommand : BaseBuiltinCommand
{
    public override string Name => "elevate";
    public override string Description => "Elevate the command prompt to administrator. (requires restart)";

    public override CommandResult Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        var ourPath = parent.GetExecutableLocation();
        var path = Path.Combine(ourPath, "Elevate.exe");

        if (!File.Exists(path))
        {
            WriteLine("This build does not appear to have `Elevate.exe`.");
            WriteLine("We expect the root project path to contain `Elevate.exe` so we can elevate.");
            return 0;
        }

        var ourProcessId = Environment.ProcessId;
        var ourProcessPath = Path.Combine(parent.GetExecutableLocation(), "Console.exe");

        var startInfo = new ProcessStartInfo
        {
            FileName = path,
            Arguments = $"-- {ourProcessPath} {ourProcessId}"
        };

        try
        {
            var proc = Process.Start(startInfo);
            proc?.WaitForExit();
        }
        catch (Exception ex)
        {
            WriteLine("failed to elevate: " + ex.ToString());
        }

        return 0;
    }

    public override string DocString => $@"
Elevate the current running process to admin.

This does not occur if the process is already administrator.
";
}
