using Console.Errors;

namespace Console.Commands.Builtins.DirBased;

public class PwdCommand : BaseBuiltinCommand
{
    public override string Name => "pwd";
    public override string Description => "Outputs the current working directory.";

    public override CommandResult Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        var workingDirectory = parent.WorkingDirectory;
        parent.EnvironmentVars.AppendCommandOutput(workingDirectory);
        parent.Ui.DisplayLinePure(workingDirectory);

        return 0;
    }

    public override string DocString => $@"
This will output the current working directory in the context
of the active terminal instance.

[bold cyan]examples[/]:
  pwd -- outputs the active working directory.
";
}
