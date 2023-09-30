using Console.Errors;

namespace Console.Commands;

public interface ICommand
{
    /// <summary>
    /// The commands name. This needs to be a name with no spaces.
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Brief description of what this command will do.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Call <see cref="Run(List{string}, Terminal)"/> on <see cref="base"/> to
    /// enable this functionality. This will allow the lastranat command to show the user
    /// when this command was last ran.
    /// </summary>
    public DateTime? LastRunTime { get; set; }

    /// <summary>
    /// The documentation for this command. This should include as much information
    /// as possible.
    /// </summary>
    public string DocString { get; }

    /// <summary>
    /// The commands entry point. See this as a main type function.
    /// </summary>
    /// <param name="args">The arguments the user has passed into this command.</param>
    /// <param name="parent">The terminal executing this command.</param>
    /// <returns></returns>
    public CommandResult Run(List<string> args, IConsole parent);
}