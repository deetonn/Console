using Console.Errors;
using System.Diagnostics;

using SystemConsole = global::System.Console;

namespace Console.Commands.Builtins.System;

public class TimeCommand : BaseBuiltinCommand
{
    // NOTES:
    // when no arguments are supplied, just output the time at the moment.
    // when arguments are present, use them to execute a command and time how long
    // it takes to execute.

    public override string Name => "time";

    public override string Description => "View the current time, or time execution.";

    public override CommandResult Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        if (args.Count == 0)
        {
            var currentTime = DateTime.Now;
            WriteLine($"{currentTime.ToLongTimeString()}");
            return 0;
        }

        var command = args.First();
        var arguments = args[1..];

        WriteSeperator();
        Stopwatch ourSw = Stopwatch.StartNew();

        Stopwatch sw = Stopwatch.StartNew();
        var result = parent.Commands.Run(command, arguments, parent);
        sw.Stop();

        TimeWriteLine(ourSw, $"{command} took {FormatElapsedTime(sw.Elapsed)} to execute.");

        if (result.IsError())
        {
            WriteSeperator();
            TimeWriteLine(ourSw, "the command returned any error, displaying it to the user.");
            WriteLine(result.GetError().GetFormatted());
            WriteSeperator();
        }

        TimeWriteLine(ourSw, $" <-- [italic]{Name}[/] done after this many milliseconds");
        WriteSeperator();

        ourSw.Stop();

        return 0;
    }

    public void TimeWriteLine(Stopwatch sw, string message)
    {
        WriteLine($"[[[italic green]{sw.ElapsedMilliseconds}[/]]] {message}");
    }

    public string FormatElapsedTime(TimeSpan ts)
    {
        if (ts.TotalSeconds > 1)
        {
            return $"{ts.TotalSeconds} seconds";
        }
        else if (ts.TotalMilliseconds > 1)
        {
            return $"{ts.TotalMilliseconds}ms";
        }
        else if (ts.TotalMicroseconds > 1)
        {
            return $"{ts.TotalMicroseconds} microseconds";
        }
        else
        {
            return $"{ts.TotalNanoseconds}ns";
        }
    }

    public void WriteSeperator()
    {
        WriteLine(new string('-', SystemConsole.BufferWidth));
    }

    public override string DocString => $@"
The time command can be used to view the current time, or time the execution of a command.

Usage:
  time [green]-- outputs the current time[/]
  time cargo run [green]this would execute `cargo run` and time how long it takes.[/]
";
}
