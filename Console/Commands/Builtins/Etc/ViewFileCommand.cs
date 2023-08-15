
using Console.Commands.Builtins.Etc.Lexer;
using Console.UserInterface;
using Pastel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;

namespace Console.Commands.Builtins.Etc;

public interface ISyntaxGenerator
{
    string Generate(string Source, string ext);
    List<GenericToken> GetTokens(string Source, string ext);
}

public class GenericSyntaxGenerator : ISyntaxGenerator
{

    public string Generate(string source, string ext)
    {
        var lexer = new GenericLexer(ext);
        var tokens = lexer.Lex(source);

        var sb = new StringBuilder();

        foreach (var tok in tokens)
        {
            var data = tok.Type switch
            {
                GenericTokenType.None => tok.Lexeme,
                GenericTokenType.Keyword => tok.Lexeme.Pastel(Color.Pink),
                GenericTokenType.String => $"\"{tok.Lexeme}\"".Pastel(Color.DarkGreen),
                GenericTokenType.Number => tok.Lexeme.Pastel(Color.Lime),
                GenericTokenType.Identifier => tok.Lexeme.Pastel(Color.LightBlue),
                GenericTokenType.FunctionCall => tok.Lexeme.Pastel(Color.LightYellow),
                GenericTokenType.Comment => tok.Lexeme.Pastel(Color.LightGreen),
                GenericTokenType.Type => tok.Lexeme.Pastel(Color.Cyan),
                _ => tok.Lexeme
            }; ;

            sb.Append(data);
        }

        return sb.ToString();
    }
    public List<GenericToken> GetTokens(string Source, string ext)
    {
        var lexer = new GenericLexer(ext);
        return lexer.Lex(Source);
    }
}

public class ViewFileCommand : BaseBuiltinCommand
{
    public override string Name => "vwf";
    public override string Description => "View a file contents within the terminal";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        try
        {
            if (args.Count < 1)
            {
                return DisplayUsage();
            }

            var showTokens = args.Contains("--show-tokens");

            if (Path.IsPathRooted(args[0]))
            {
                var path0 = Path.GetFullPath(args[0]);
                if (!File.Exists(path0))
                {
                    WriteLine($"No such file '{path0}'");
                    return -1;
                }
                var contents0 = File.ReadAllText(path0);
                var info0 = new FileInfo(path0);
                return DisplayFileContents(contents0, info0.Extension, parent, showTokens);
            }

            var path = Path.Combine(parent.WorkingDirectory, args[0]);
            if (!File.Exists(path))
            {
                WriteLine($"No such file '{path}'");
                return -1;
            }

            var contents = File.ReadAllText(path);
            var info = new FileInfo(path);
            return DisplayFileContents(contents, info.Extension, parent, showTokens);
        }
        catch (Exception ex)
        {
            WriteLine($"Failed to read the file [{ex.Message}]");
        }

        return -1;
    }

    private int DisplayUsage()
    {
        WriteLine($"USAGE -- {Name}");
        WriteLine($"{Name} <file-name>");
        WriteLine($"Options:");
        WriteLine("  --show-tokens: display the generated lexed tokens instead of text");

        return -1;
    }

    private int DisplayFileContents(string contents, string ext, IConsole console, bool showTokens = false)
    {
        var lines = contents.Split('\n');
        ISyntaxGenerator syntaxHighlighter;

        if (ext == ".DL(WORKINPROGRESS)")
        {
            syntaxHighlighter = new DeeLHighlighter();
        }
        else
        {
            syntaxHighlighter = new GenericSyntaxGenerator();
        }

        int tokenCount = 0;

        for (int i = 0; i < lines.Length; ++i)
        {
            var indent = i + 1 < 10 ? " " : "";

            if (!showTokens)
            {
                var line = syntaxHighlighter.Generate(lines[i], ext);
                var lno = i + 1;
                console.Ui.DisplayLinePure($"{lno}{indent}| {line}");
                continue;
            }
            else
            {
                var tokens = syntaxHighlighter.GetTokens(lines[i], ext);
                foreach (var token in tokens)
                {
                    WriteLine(token.ToString());
                }

                tokenCount += tokens.Count;
            }
        }

        if (showTokens)
        {
            WriteLine($"\n{tokenCount} tokens in total.");
        }

        return 0;
    }

    public override string DocString => $@"
This command will display the contents of a file.
If the file is a source code file, it will be syntax highlighted.

USAGE: {Name} <file-name> [...options]

Options:
  --show-tokens: display the generated lexed tokens instead of text

Example:
  {Name} test.cpp
  {Name} test.c --show-tokens

If the file is not found, the command will fail.
The argument can be relative to the current directory, or absolute.
";
}
