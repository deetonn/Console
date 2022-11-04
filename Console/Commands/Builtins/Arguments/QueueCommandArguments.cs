using CommandLine;

namespace Console.Commands.Builtins;

internal class QueueCommandArguments
{
    [Option('n', "name")] 
    public string CommandName { get; set; } = null!;
    
    [Option('b', "begin")]
    public bool WantsQueue { get; set; }
    
    [Option('d', "dequeue")]
    public bool WantsDequeue { get; set; }
    
    [Option('a', "arguments")]
    public IEnumerable<string>? DequeueArgs { get; set; }
}