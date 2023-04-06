using CommandLine;

namespace Console.Commands.Builtins.Arguments;

public class LineCountCommandArguments
{
    [Option('i', "ignored-exts", HelpText = "File extensions to ignore")]
    public IEnumerable<string>? IgnoredExtensions { get; set; }

    [Option('d', "directory", Required = true, HelpText = "The directory to count the lines of.")]
    public string Path { get; set; } = null!;

    [Option('r', "recursive", HelpText = "Recurse the directorys within the parent directory.")]
    public bool Recursive { get; set; }

    [Option('v', "verbose", HelpText = "Enable verbose output")]
    public bool Verbose { get; set; }
} 
