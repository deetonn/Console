using Console.Errors;

namespace Console.Commands.Builtins.System;

public class SetCommand : BaseBuiltinCommand
{
    public override string Name => "set";

    public override string Description => "set an environment variable.";

    public override CommandResult Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        // syntax: set <env-name> <value>

        if (args.Count < 2)
        {
            return Error()
                .WithMessage("invalid amount of arguments supplied.")
                .WithNote($"expected (< 2) but got {args.Count}")
                .WithNote($"use \"docs {Name}\" for more information.")
                .Build();
        }

        var name = args[0];
        var value = args[1];

        if (parent.EnvironmentVars.Get(name) is null)
        {
            return Error()
                .WithMessage($"no such environment variable \"{name}\"")
                .Build();
        }

        parent.EnvironmentVars.Set(name, value);

        return 0;
    }

    public override string DocString => $@"
This command will set an environment variable in the current processes context.

USAGE: {Name} <env-name> <value>
EXAMPLES:
    set the path to the current path + /bin on windows.
       set PATH {{PATH}};/bin
    set the path to /bin on any os.
       set PATH /bin
";
}
