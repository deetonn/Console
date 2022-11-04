namespace Console.Commands.Builtins;

public class CuntCommand : BaseBuiltinCommand
{
    public override string Name => "Cunt";
    public override string Description => "Get called a cunt!";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, Terminal parent)
    {
        base.Run(args, parent);

        var name = Environment.UserName;
        parent.Ui.DisplayLine($"You, {name}, are a cunt!");

        return 1;
    }
}