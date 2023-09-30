using Console.Errors;

namespace Console.Commands.Builtins;

public class ChangeDirectoryCommand : BaseBuiltinCommand
{
    public override string Name => "cd";
    public override string Description => "Change the active directory";
    public override DateTime? LastRunTime { get; set; } = null;
    public override CommandResult Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        if (args.Count != 1)
        {
            // If the user just does "cd" then just walk back a directory.
            // This saves actually typing "cd .., cd .., cd ..
            return Run(new() { ".." }, parent);
        }

        var path = args.First();

        if (path.Contains('~'))
        {
            path = path.Replace("~", Environment.GetFolderPath(
                Environment.SpecialFolder.UserProfile));
        }

        if (string.IsNullOrEmpty(path))
        {
            return Error()
                .WithMessage("cannot set the working directory to an empty string.")
                .WithNote("you can only transition into real directorys (???)")
                .Build();
        }

        try
        {
            Environment.CurrentDirectory = path;
        }
        catch (Exception ex)
        {
            return Error()
                .WithMessage("an exception occured while trying to change directory.")
                .WithNote($"message: {ex.Message}")
                .Build();
        }

        parent.WorkingDirectory = Environment.CurrentDirectory;

        return 0;
    }

    public override string DocString => $@"
This command will change the active directory. This current active directory can be
seen in the prompt.

This command accepts a relative path, which will be relative to the current active directory.
It also accepts rooted paths, which will change the directory entirely.

The ../.. syntax is supported, along with ./ syntax.
You can use a `~` to navigate to the current users home directory.

Example usage:
  cd ..
  cd ./Desktop
  cd ~/Documents
  cd C:/Windows

This is a core command. It cannot be unloaded.
";
}