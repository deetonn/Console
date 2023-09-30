using Console.Errors;

namespace Console.Commands.Builtins.System;

internal class QuitCommand : BaseBuiltinCommand
{
    public override string Name => "quit";

    public override string Description => "Exit the application with proper cleanup.";

    public override CommandResult Run(List<string> args, IConsole parent)
    {
        return CommandReturnValues.SafeExit;
    }

    public override string DocString => $@"
This command will cause the application to exit.
This exit operation will cleanup all resources and perform a graceful shutdown.
";
}
