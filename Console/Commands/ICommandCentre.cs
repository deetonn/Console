namespace Console.Commands;

public interface ICommandCentre
{
    /// <summary>
    /// All commands. For commands that are loaded from
    /// the `$PATH` environment variable, they should have
    /// their own implementation of ICommand
    /// </summary>
    public IList<ICommand> Elements { get; }
    
    public IList<ICommand> PausedCommands { get; }

    /// <summary>
    /// Execute a command. This command must be located
    /// within <see cref="Elements"/>. Any external commands not
    /// present here, should not be executed.
    /// </summary>
    /// <param name="name">The name of the command</param>
    /// <param name="args">The arguments passed by the user</param>
    /// <param name="owner">The parent terminal owning this command</param>
    /// <returns>The command result</returns>
    public int Run(string name, List<string> args, Terminal owner);
    
    /// <summary>
    /// Load all types that derive from <see cref="ICommand"/>.
    /// </summary>
    /// <returns>All, if any, instances of <see cref="ICommand"/></returns>
    public List<ICommand> LoadBuiltinCommands();

    /// <summary>
    /// Load all executables from PATH folders.
    /// </summary>
    /// <returns>All, if any, instances </returns>
    public List<ICommand> LoadPathExecutables();

    public PathFileCommand? FinishQueuedCommand(string command);

    public int AttemptToQueueCommand(string name, List<string> args, Terminal owner);
}