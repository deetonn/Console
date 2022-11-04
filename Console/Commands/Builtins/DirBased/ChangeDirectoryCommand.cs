using System.Security;

namespace Console.Commands.Builtins;

public class ChangeDirectoryCommand : BaseBuiltinCommand
{
    public override string Name => "Cd";
    public override string Description => "Change the active directory";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, Terminal parent)
    {
        base.Run(args, parent);

        if (args.Count != 1)
        {
            return CommandReturnValues.BadArguments;
        }

        var path = args.First();

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
}