
namespace Console.Commands.Builtins.Etc;

public class ClearBufferCommand : BaseBuiltinCommand
{
    public override string Name => "clear";
    public override string Description => "Clear the Terminal buffer";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, Terminal parent)
    {
        base.Run(args, parent);

        parent.Ui.Clear();

        return 1;
    }
}
