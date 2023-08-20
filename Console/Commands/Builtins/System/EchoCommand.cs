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

    public override string DocString => $@"
This command allows you to output text onto the terminal interface.

Markup using Spectre.Console is supported.

USAGE: {Name} <text...>
EXAMPLES:
  Output the text ""hello world""
    {Name} hello world
  Output my name is joe, with joe being [italic]italic[/]
    {Name} my name is [[italic]]joe[[/]]
  Output the path environment variable all [red]red[/]
    {Name} [[red]]{{PATH}}[[/]]
";
}
