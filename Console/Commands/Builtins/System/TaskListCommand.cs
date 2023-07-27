using System.ComponentModel;
using System.Diagnostics;

namespace Console.Commands.Builtins.System;

public class TaskListCommand : BaseBuiltinCommand
{
    public override string Name => "tasklist";
    public override string Description => "List all running processes on the system";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, Terminal parent)
    {
        base.Run(args, parent);

        var processes = Process.GetProcesses();

        foreach (var process in processes)
        {
            try
            {
                WriteLine($"[{process.Id}] {process.ProcessName}");
            }
            catch (Win32Exception w32)
            {
                WriteLine($"Failed to display process '{process.ProcessName}' ({w32.Message})");
            }
        }

        return CommandReturnValues.DontShowText;
    }

    public override string DocString => $@"
This command will display a list of all running processes on this machine and their
process ID.

This command does not take any arguments.
";
}
