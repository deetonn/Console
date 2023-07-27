namespace Console.Commands.Builtins;

public class DirCommand : BaseBuiltinCommand
{
    public override string Name => "dir";
    public override string Description => "Query the active directory";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, Terminal parent)
    {
        base.Run(args, parent);

        var activeDirectory = parent.WorkingDirectory;
        var info = new DirectoryInfo(activeDirectory);

        if (!info.Exists)
        {
            WriteLine("The active directory does not exist.");
            return -2;
        }

        var childrenFiles = info.GetFiles();
        var childrenFolders = info.GetDirectories();

        var parentFolder = (info.Parent != null) ? info.Parent.Name : "root"; 
        
        WriteLine($"Parent: {parentFolder}\n");
        foreach (var folder in childrenFolders)
        {
            var fmt = string.Format("{0,7} {1,7} {2,7} {3,7}", 
                folder.LastAccessTimeUtc, "<DIR>", "", folder.Name);
            WriteLine(fmt);
        }

        foreach (var file in childrenFiles)
        {
            var fmt = string.Format("{0,7} {1,7} {2,7} {3,7}", 
                file.LastAccessTimeUtc, "", $"{file.Length}", file.Name);
            WriteLine(fmt);
        }
        
        return 1;
    }

    public override string DocString => $@"
This command will output information about the files & directorys in the
current directory. It also accepts an argument which if present will output
information about that directory instead.

If the supplied directory does not exist, an error will be displayed.

Example usage:
  dir
  dir C:\Windows

The information displayed is:
  - Last access time
  - File size (if applicable)
  - Directory size (if applicable)
  - Name
";
}