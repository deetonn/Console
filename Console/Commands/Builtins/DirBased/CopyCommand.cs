using Console.Errors;

namespace Console.Commands.Builtins.DirBased;

public class CopyCommand : BaseBuiltinCommand
{
    // override base class members
    public override string Name => "copy";
    public override string Description => "Copies a file or directory to another location.";
    public override CommandResult Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        // copy expects at least two arguments
        // the syntax would be `copy <src> <dst> [options]
        if (args.Count < 2)
        {
            return Error()
                .WithMessage("invalid usage. expected at least 2 arguments.")
                .WithNote("usage: copy <src> <dst> [[options...]]")
                .WithNote("src: The source file/directory")
                .WithNote("dst: The destination file/directory.")
                .WithNote($"use \"docs {Name}\" for more information.")
                .Build();
        }

        // get the source and destination paths
        var src = args[0];
        var dst = args[1];

        if (Directory.Exists(src))
        {
            return CopyDirectory(src, dst);
        }
        else
        {
            return CopyFile(src, dst);
        }
    }

    public int CopyDirectory(string directory, string dst)
    {
        // This function will copy a directory and all of its contents
        // to src
        // If the directory already exists, it will be overwritten
        // If the directory does not exist, it will be created
        // If the directory does not exist, an error will be displayed

        // Verify the destination directory exists
        if (!Directory.Exists(directory))
        {
            WriteLine($"copy: {directory}: No such file or directory");
            return 1;
        }

        // It's already verified that the directory exists & that it's a 
        // directory due to the callee checking this before calling.
        // So we can safely assume that this is a directory.
        // We need to get the directory name from the source and append it
        // to the destination
        var dirname = Path.GetFileName(directory);
        dst = Path.Combine(dst, dirname);
        if (!Directory.Exists(dst))
        {
            // create it
            Directory.CreateDirectory(dst);
        }

        // get all of the files in the directory
        var files = Directory.GetFiles(directory);
        foreach (var file in files)
        {
            // copy each file
            CopyFile(file, dst);
        }

        // get all of the directories in the directory
        var directories = Directory.GetDirectories(directory);
        foreach (var dir in directories)
        {
            // copy each directory
            CopyDirectory(dir, dst);
        }
        return 0;
    }

    public int CopyFile(string file, string dest)
    {
        // This function will attempt to copy a file into a new directory
        // If the file already exists, it will be overwritten
        // If the directory does not exist, it will be created
        // If the file does not exist, an error will be displayed

        // check if the file exists
        if (!File.Exists(file))
        {
            WriteLine($"copy: {file}: No such file or directory");
            return 1;
        }

        // check if the destination is a directory
        if (Directory.Exists(dest))
        {
            // if the destination is a directory, we need to get the filename
            // from the source and append it to the destination
            var filename = Path.GetFileName(file);
            dest = Path.Combine(dest, filename);
        }
        else
        {
            // create the directory
            Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
        }

        // copy the file
        File.Copy(file, dest, true);
        // return a success error code
        return 0;
    }

    // Document the above code in this string
    override public string DocString => $@"
This command will copy a file or directory to another location.

If the destination is a directory, the file will be copied into the directory.
If the destination is a file, the file will be copied over the destination file.
If the destination is a directory, and the source is a directory, the source directory will be copied into the destination directory.

If the source does not exist, an error will be displayed.
If the destination does not exist, it will be created.

Usage:
    copy <src> <dst> [[options...]]
";
}
