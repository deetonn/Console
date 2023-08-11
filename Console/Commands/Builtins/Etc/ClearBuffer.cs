
namespace Console.Commands.Builtins.Etc;

public class ClearBufferCommand : BaseBuiltinCommand
{
    public override string Name => "clear";
    public override string Description => "Clear the Terminal buffer";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        parent.Ui.Clear();

        return 1;
    }

    public override string DocString => $@"
This command will clear the Terminal buffer.
";
}
