namespace Console.Commands.Builtins.System;

public class SetCommand : BaseBuiltinCommand
{
    public override string Name => "set";

    public override string Description => "set an environment variable.";

    public override int Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        // syntax: set <env-name> <value>

        if (args.Count < 2)
        {
            WriteError($"[[[red]error[/]]] invalid arguments supplied. USAGE: {Name} <env-name> <value>");
            return -1;
        }

        var env_variable_name = args[0];
        var env_variable_new_val = args[1];

        if (Environment.GetEnvironmentVariable(env_variable_name) is null)
        {
            WriteError($"no such environment variable: `{env_variable_name}`");
            return -1;
        }

        try
        {
            Environment.SetEnvironmentVariable(env_variable_name, env_variable_new_val);
        }
        catch (Exception e)
        {
            WriteError(e.Message);
            return -1;
        }

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
