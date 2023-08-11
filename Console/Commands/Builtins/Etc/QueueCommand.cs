using CommandLine;
using CommandLine.Core;

namespace Console.Commands.Builtins;

public class QueueCommand : BaseBuiltinCommand
{
    public override string Name => "queue";
    public override string Description => "Queue a command to run when asked to.";
    public override DateTime? LastRunTime { get; set; } = null;

    public readonly bool Deprecated = true;

    public override int Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        if (Deprecated)
        {
            WriteLine("This command is deprecated.\n"
                + "It may be re-enabled once more functionality is added.");
            return -1;
        }

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

    private int DoQueue(string name, List<string> args, IConsole parent)
    {
        return parent.Commands.AttemptToQueueCommand(name, args, parent);
    }

    private int DoDequeue(List<string> args, string name, IConsole parent)
    {
        var thing = parent.Commands.FinishQueuedCommand(name);

        if (thing == null)
        {
            return CommandReturnValues.CQueueNotFound;
        }
        
        if (thing is PathFileCommand pfc)
        {
            return pfc.ResumeExecution(args, parent);
        }

        return thing.Run(args, parent);
    }

    public override string DocString => $@"
This command is deprecated. The queue functionality has been removed.
";
}