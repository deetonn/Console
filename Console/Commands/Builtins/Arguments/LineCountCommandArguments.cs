using CommandLine;

namespace Console.Commands.Builtins.Arguments;

public class LineCountCommandArguments
{
    [Option('V', "valid-exts", HelpText = "File extensions to ignore")]
    public IEnumerable<string>? ValidExtensions { get; set; }

    [Option('d', "directory", HelpText = "The directory to count the lines of.")]
    public string Path { get; set; } = null!;

    [Option('r', "recursive", HelpText = "Recurse the directorys within the parent directory.")]
    public bool Recursive { get; set; }

    [Option('v', "verbose", HelpText = "Enable verbose output")]
    public bool Verbose { get; set; }

    [Option('f', "file-name", HelpText = "The singular file you want to count the lines of.")]
    public string? FileName { get; set; }

    [Option('P', "preset", HelpText = "Choose a language preset. This matches the languages natural extension. For example, C++ would be `cpp`. CSharp would be `cs` etc...")]
    public string? Preset { get; set; } = null;
} 
