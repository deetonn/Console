using CommandLine;

namespace Console.Commands.Builtins.Arguments;

public class LineCountCommandArguments
{
    [Option('V', "valid-exts", HelpText = "File extensions to ignore")]
    public IEnumerable<string>? ValidExtensions { get; set; }

    [Option('d', "directory", HelpText = "The directory to count the lines of.")]
    public string Path { get; set; } = "INVALID";

    [Option('r', "recursive", HelpText = "Recurse the directorys within the parent directory.")]
    public bool Recursive { get; set; }

    [Option('v', "verbose", HelpText = "Enable verbose output")]
    public bool Verbose { get; set; }

    [Option('f', "file-name", HelpText = "The singular file you want to count the lines of.")]
    public string? FileName { get; set; } = "INVALID";

    [Option('P', "preset", HelpText = "Choose a language preset. This matches the languages natural extension. For example, C++ would be `cpp`. CSharp would be `cs` etc...")]
    public string? Preset { get; set; } = "INVALID";

    public List<string> IntoOriginal()
    {
        List<string> original = new();

        if (ValidExtensions is not null)
        {
            original.Add($"-V {string.Join(", ", ValidExtensions)}");
        }

        original.Add($"-d {Path}");

        if (Recursive)
        {
            original.Add("-r");
        }

        if (Verbose)
            original.Add("-v");

        if (!string.IsNullOrEmpty(FileName))
        {
            original.Add($"-f {FileName}");
        }

        if (!string.IsNullOrEmpty(Preset))
        {
            original.Add($"-P {Preset}");
        }

        return original;
    }
} 
