
using CommandLine;
using Console.Commands.Builtins.Arguments;
using System.ComponentModel;
using System.Diagnostics;

namespace Console.Commands.Builtins.DirBased;

public class LineCountCommand : BaseBuiltinCommand
{
    public override string Name => "linec";
    public override string Description => "View the total line count of all files within a directory.";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, Terminal parent)
    {
        base.Run(args, parent);

        var arguments
            = Parser.Default
            .ParseArguments<LineCountCommandArguments>(args)
            .Value;

        if (arguments is null)
            return -1;

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
                if (arguments.IgnoredExtensions?.Contains(info.Extension) ?? false
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

        var recursed = (arguments.Recursive) ? "Recursive" : "Not Recursive";
        WriteLine($"Total: {lineCount} lines in total. ({recursed}, {files.Length - filesSkipped})");
        WriteLine($"Skipped {filesSkipped} files because of filters.");

        return CommandReturnValues.DontShowText;
    }

    ulong ReadFileTotal(string fileContents)
    {
        return (ulong)fileContents.Split('\n').Length;
    }
}
