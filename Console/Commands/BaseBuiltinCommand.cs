using Console.UserInterface;
using Console.UserInterface.UiTypes;

namespace Console.Commands;

public class BaseBuiltinCommand : ICommand
{
    private IConsole? _terminal;
    
    public virtual string Name { get; } = null!; 
    
    public virtual string Description { get; } = null!;
    
    public virtual DateTime? LastRunTime { get; set; } = default;

    public virtual string DocString { get; protected set; } = "";

    public virtual int Run(List<string> args, IConsole parent)
    {
        LastRunTime = DateTime.Now;
        _terminal = parent;
        var args_str = 
            args.Count == 0 ?
            "no arguments"
            : "[" + string.Join(", ", args) + "]";
        Logger().LogInfo(this, $"`{Name}` is executing with `{args_str}` under terminal `{parent}`");
        return 0;
    }
    
    /// <summary>
    /// Helper methods, will write to the parents output stream.
    /// Must call <see cref="Run"/> from base before using.
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="severity">The messages severity</param>
    protected void WriteLine(string message)
    {
        _terminal?.Ui.DisplayLineMarkup($"[cyan]{Name}[/]: " + message);
    }

    protected void WriteError(string message)
    {
        WriteLine($"[[[red]error[/]]]: {message}");
    }
    
    /// <summary>
    /// Helper methods, will write to the parents output stream.
    /// Must call <see cref="Run"/> from base before using.
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="severity">The messages severity</param>
    protected void Write(string message)
    {
        _terminal?.Ui.DisplayMarkup($"[cyan]{Name}[/]: " + message);
    }

    /// <summary>
    /// Helper methods, read input from the user. This support markup.
    /// </summary>
    /// <param name="prompt">The markup string to present, if null <see cref="string.Empty"/> is used.</param>
    /// <returns>The string if successful.</returns>
    protected string? ReadLine(string? prompt = null)
    {
        if (prompt == null)
        {
            return _terminal?.Ui.GetLine(string.Empty);
        }

        return _terminal?.Ui.GetLine(prompt);
    }

    public virtual void OnInit(IConsole parent)
    {
        // called when the command is loaded.
    }
}