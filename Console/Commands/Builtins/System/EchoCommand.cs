namespace Console.Commands.Builtins.System;

public class EchoCommand : BaseBuiltinCommand
{
    public override string Name => "echo";
    public override string Description => "Prints the specified text to the console. (supports markdown using AnsiConsole)";
    public override int Run(List<string> args, IConsole parent)
    {
        var joined = string.Join(" ", args);
        try
        {
            parent.Ui.DisplayLineMarkup(joined);
        }
        catch (Exception e)
        {
            WriteLine($"failed due to markdown error. ({e.Message})");
        }
        return CommandReturnValues.DontShowText;
    }
}
