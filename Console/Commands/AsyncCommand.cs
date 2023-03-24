
namespace Console.Commands;

/// <summary>
/// This implementation serves as a way
/// to queue code-based commands.
/// </summary>
public class AsyncCommand : ICommand
{
    public AsyncCommand(ICommand command)
    {
        _wrapper = command;
    }

    private readonly ICommand _wrapper;

    public string Name => _wrapper.Name;

    public string Description => _wrapper.Description;

    public DateTime? LastRunTime { get => _wrapper.LastRunTime; set => _wrapper.LastRunTime = value; }

    public int Run(List<string> args, Terminal parent)
    {
        return _wrapper.Run(args, parent);
    }
}
