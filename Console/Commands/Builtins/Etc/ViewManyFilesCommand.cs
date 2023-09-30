using Console.Errors;

namespace Console.Commands.Builtins.Etc;

public class ViewManyFilesCommand : BaseBuiltinCommand
{
    public override string Name => "vwmf";
    public override string Description => "View each code file in a directory.";
    public override DateTime? LastRunTime { get; set; } = null;
    public override CommandResult Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        var cwd = args.Any()
            ? args[0]
            : parent.WorkingDirectory;

        if (!Directory.Exists(cwd))
        {
            return Error()
                .WithMessage("invalid argument")
                .WithNote($"the directory \"{cwd}\" does not exist.")
                .Build();
        }

        var files = Directory.GetFiles(cwd, "*", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            _ = parent.Commands.Run(
                "vwf",
                new List<string>()
                {
                    file
                },
                parent);
            WriteLine("Press any key to go next...");
            _ = parent.Ui.GetKey();
        }

        return CommandReturnValues.DontShowText;
    }

    public override string DocString => $@"
This command will display each file in a directory, one by one.

It uses the `vwf` command to display each file.

The directory can be relative or absolute.
";
}
