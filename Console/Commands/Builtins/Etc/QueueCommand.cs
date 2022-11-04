using CommandLine;
using CommandLine.Core;

namespace Console.Commands.Builtins;

public class QueueCommand : BaseBuiltinCommand
{
    public override string Name => "Queue";
    public override string Description => "Queue a command to run when asked to.";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, Terminal parent)
    {
        base.Run(args, parent);

        var arguments =
            Parser.Default
                .ParseArguments<QueueCommandArguments>(args)
                .Value;

        if (arguments is null)
        {
            return CommandReturnValues.BadArguments;
        }

        if (arguments.WantsQueue)
        {
            var commandArgs = (arguments.DequeueArgs != null)
                ? arguments.DequeueArgs.ToList()
                : Array.Empty<string>().ToList();
            
            return DoQueue(arguments.CommandName, commandArgs, parent);
        }

        if (arguments.WantsDequeue)
        {
            var commandArgs = (arguments.DequeueArgs != null)
                ? arguments.DequeueArgs.ToList()
                : Array.Empty<string>().ToList();

            return DoDequeue(commandArgs, arguments.CommandName, parent);
        }

        // list all active queued commands.
       
        WriteLine($"There is currently {parent.Commands.PausedCommands.Count} queued.\n");
        foreach (var queuedCommand in parent.Commands.PausedCommands)
        {
            WriteLine($"{queuedCommand.Name} -- Queued since {queuedCommand.LastRunTime?.ToShortTimeString()}");
        }

        return 0;
    }

    private int DoQueue(string name, List<string> args, Terminal parent)
    {
        return parent.Commands.AttemptToQueueCommand(name, args, parent);
    }

    private int DoDequeue(List<string> args, string name, Terminal parent)
    {
        var thing = parent.Commands.FinishQueuedCommand(name);

        if (thing == null)
        {
            return CommandReturnValues.CQueueNotFound;
        }
        
        return thing.ResumeExecution(args, parent);
    }
}