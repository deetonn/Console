
using CommandLine;
using Console.Commands.Builtins.Arguments;
using System.ComponentModel;
using System.Diagnostics;

namespace Console.Commands.Builtins.DirBased;

public class LineCountCommand : BaseBuiltinCommand
{
    public readonly Dictionary<string, LineCountCommandArguments> Presets = new()
    {
        ["cpp"] = new()
        {
            Path = ".",
            ValidExtensions = new[] { ".cpp", ".hpp", ".h", ".c", ".cc", ".cxx", ".hxx" },
            Recursive = true,
            Verbose = true
        },
        ["c"] = new()
        {
            Path = ".",
            ValidExtensions = new[] { ".c", ".h" },
            Recursive = true,
            Verbose = true
        },
        ["cs"] = new()
        {
            Path = ".",
            ValidExtensions = new[] { ".cs" },
            Recursive = true,
            Verbose = true
        },
        ["py"] = new()
        {
            Path = ".",
            ValidExtensions = new[] { ".py", ".pyw" },
            Recursive = true,
            Verbose = true
        },
        ["rs"] = new()
        {
            Path = ".",
            ValidExtensions = new[] { ".rs" },
            Recursive = true,
            Verbose = true
        }
    };

    public override string Name => "linec";
    public override string Description => "View the total line count of all files within a directory.";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        var arguments
            = Parser.Default
            .ParseArguments<LineCountCommandArguments>(args)
            .Value;

        if (arguments is null)
            return -1;

        if (arguments.Preset is not null)
        {
            if (!Presets.ContainsKey(arguments.Preset))
            {
                WriteLine($"No such preset `{arguments.Preset}`");
                var validPresets = Presets.Keys;
                WriteLine($"Valid presets: {string.Join(", ", validPresets)}");
                return -1;
            }

            arguments = Presets[arguments.Preset];
        }

        if (arguments.FileName is not null)
        {
            var path = arguments.FileName;

            if (!File.Exists(arguments.FileName))
            {
                path = Path.Combine(parent.WorkingDirectory, arguments.FileName);
                if (!File.Exists(path))
                {
                    WriteLine($"No such file exists.");
                    return -1;
                }
            }

            var contents = File.ReadAllLines(path);
            var count = contents.Length;
            WriteLine($"Line count: {count}");
            return count;
        }

        var directory = arguments.Path;
        if (!Directory.Exists(directory))
        {
            var full = Path.Combine(parent.WorkingDirectory, directory);

            if (!Directory.Exists(full))
            {
                WriteLine($"No such file or directory '{directory}'");
                return -1;
            }

            directory = full;
        }

        SearchOption options =
            arguments.Recursive ? SearchOption.AllDirectories
            : SearchOption.TopDirectoryOnly;
        string[] files;

        try
        {
            files = Directory.GetFiles(directory, "*.*", options);
        }
        catch (Exception ex)
        {
            WriteLine($"Failure. Cannot enumerate folders. [{ex.Message}]");
            return -1;
        }

        ulong lineCount = 0;
        ulong filesCounted = 0;
        var filesSkipped = 0;

        foreach (var file in files)
        {
            try
            {
                filesCounted++;

                var info = new FileInfo(file);
                if (!arguments.ValidExtensions?.Contains(info.Extension) ?? false
                    || info.Extension == "")
                {
                    filesSkipped++;
                    continue;
                }
                lineCount += (ulong)File.ReadAllLines(file).LongLength;

                if (arguments.Verbose)
                {
                    global::System.Console.Clear();
                    WriteLine($"[{filesCounted}/{files.Length}] {file}");
                }
            }
            catch (Exception ex)
            {
                WriteLine($"ERROR: {ex.Message}");
            }
        }

        var recursed = arguments.Recursive ? "Recursive" : "Not Recursive";
        WriteLine($"Total: {lineCount} lines in total. ({recursed}, {files.Length - filesSkipped})");
        WriteLine($"Skipped {filesSkipped} files because of filters.");

        return (int)lineCount;
    }

    public override string DocString => $@"
This command will iterate a directory, counting all applicable files and their lines.
It can also apply to one file if requested.

The purpose of this command is for programmers, counting the line count in their projects.

Usage:
  -V, --valid-exts: The valid extensions to count.
  -d, --directory: The directory to count files in.
  -r, --recursive: Whether to count files recursively, if present this will count any files within directorys
                   of the current directory supplied by --directory.
  -v, --verbose: Whether to show the current file being counted.
  -f, --file: Whether to count a single file instead of a directory, this and
              --directory cannot be supplied together.
  -P, --preset: The file extension preset to use. (cpp, c, cs, py, rs)
                This option will override all other options. It will set
                the valid extensions to any extension related to that programming
                language. It will also set the directory to the current working
                directory. It will recurse & verbose is enabled.

This command is very useful for programmers, counting the line count in their projects.
";
}
