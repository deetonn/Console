namespace Console.Commands.Builtins;

public class AboutCommand : BaseBuiltinCommand
{
    public override string Name => "about";
    public override string Description => "About Console";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);
        
        WriteLine("Terminal is a project, made for fun.");
        WriteLine("It attempts to emulate Windows cmd.");
        WriteLine("While also keeping the nice feel of bash alive.");

        WriteLine($"This project can be found at [link={Terminal.GithubLink}]this github repo[/]. It is open source.");

        return CommandReturnValues.DontShowText;
    }

    public override string DocString => $@"
This command will display some basic information about this application.
";
}