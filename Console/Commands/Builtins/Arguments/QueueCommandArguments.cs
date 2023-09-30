using CommandLine;

namespace Console.Commands.Builtins;

internal class QueueCommandArguments
{
    [Option('n', "name", HelpText = "The name of the command to queue.")]
    public string CommandName { get; set; } = null!;

    [Option('b', "begin", HelpText = "Queue the command?")]
    public bool WantsQueue { get; set; }

    [Option('d', "dequeue", HelpText = "Dequeue the command?")]
    public bool WantsDequeue { get; set; }

    [Option('a', "arguments", HelpText = "Arguments to pass when calling the command.")]
    public IEnumerable<string>? DequeueArgs { get; set; }
}