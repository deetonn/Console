namespace Console.Commands.Builtins;

public class AboutCommand : BaseBuiltinCommand
{
    public override string Name => "About";
    public override string Description => "About Console";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, Terminal parent)
    {
        base.Run(args, parent);
        
        WriteLine("Terminal is a project, made for fun.");
        WriteLine("It attempts to emulate Windows cmd.");
        WriteLine("While also keeping the nice feel of bash alive.");

        return CommandReturnValues.DontShowText;
    }
}