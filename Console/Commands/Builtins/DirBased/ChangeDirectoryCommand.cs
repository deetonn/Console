using System.Security;

namespace Console.Commands.Builtins;

public class ChangeDirectoryCommand : BaseBuiltinCommand
{
    public override string Name => "cd";
    public override string Description => "Change the active directory";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        if (args.Count != 1)
        {
            return CommandReturnValues.BadArguments;
        }

        var path = args.First();

        if (path.Contains('~'))
        {
            path = path.Replace("~", Environment.GetFolderPath(
                Environment.SpecialFolder.UserProfile));
        }

        if (string.IsNullOrEmpty(path))
        {
            WriteLine("Cannot set the working directory to an empty string.");
            return CommandReturnValues.BadArguments;
        }

        try
        {
            Environment.CurrentDirectory = path;
        }
        catch (DirectoryNotFoundException ex)
        {
            WriteLine($"{ex.Message}");
            return CommandReturnValues.BadArguments;
        }
        catch (SecurityException ex)
        {
            WriteLine($"{ex.Message}");
            return -1;
        }
        catch (UnauthorizedAccessException ex)
        {
            // same as above, for some reason SecurityException is not thrown?
            WriteLine($"{ex.Message}");
            return -1;
        }
        
        parent.WorkingDirectory = Environment.CurrentDirectory;
        
        return 1;
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