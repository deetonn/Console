using Console.Errors;
using Spectre.Console;

namespace Console.Commands.Builtins.Etc;

public class LogCommand : BaseBuiltinCommand
{
    public override string Name => "log";
    public override string Description => "view the logs for this session.";

    public override CommandResult Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        var configPath = parent.GetConfigPath();
        var logFile = Path.Combine(configPath, "log.txt");

        if (!File.Exists(logFile))
        {
            return Error()
                .WithMessage("The log file no longer exists.")
                .WithNote("This is unlikely to be a bug, have you recently tampered with the logfile?")
                .Build();
        }

        var logs = File.ReadAllLines(logFile);

        foreach (var line in logs)
        {
            WriteLine($"LOG | {line.EscapeMarkup()}");
        }

        return 0;
    }
}
