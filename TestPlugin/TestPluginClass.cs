using Console.Commands;
using Console.Errors;

namespace Console;

public class TestCommand : BaseBuiltinCommand
{
    public override string Name => "test";
    public override string Description => "This command exists for testing purposes";

    public override CommandResult Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        WriteLine("This is from the externally loaded test command!");

        return 0;
    }
}