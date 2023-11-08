
using Console.Errors;

namespace Console.Commands.Builtins.Informational;

public class ShowEnv : BaseBuiltinCommand
{
    public override string Name => "env";
    public override string Description => "perform operations on the environment.";

    public override CommandResult Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        if (args.Count == 0)
        {
            foreach (var variable in parent.EnvironmentVars.Variables)
            {
                if (variable.Key == "path")
                {
                    // format path differently
                    WriteLine("Path:");
                    foreach (var p in variable.Value.Split(Terminal.PathSep))
                    {
                        WriteLine($" * [white]{p}[/]");
                    }
                    continue;
                }

                WriteLine($"[white]{variable.Key}[/]: [italic blue]{variable.Value}[/]");
            }

            return 0;
        }

        // TODO: add --remove (env --remove Path)
        // TODO: add --add    (env --add Key Value)
        // TODO: add --reload (env --reload)

        return 0;
    }
}
