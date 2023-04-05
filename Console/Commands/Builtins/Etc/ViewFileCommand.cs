
using Console.Commands.Builtins.Etc.Lexer;
using Console.UserInterface;
using Pastel;
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
    public override int Run(List<string> args, Terminal parent)
    {
        base.Run(args, parent);

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
            return DisplayFileContents(contents0, info0.Extension, showTokens);
        }

        var path = Path.Combine(parent.WorkingDirectory, args[0]);
        if (!File.Exists(path))
        {
            WriteLine($"No such file '{path}'");
            return -1;
        }

        var contents = File.ReadAllText(path);
        var info = new FileInfo(path);
        return DisplayFileContents(contents, info.Extension, showTokens);
    }

    private int DisplayUsage()
    {
        WriteLine($"USAGE -- {Name}");
        WriteLine($"{Name} <file-name>");
        WriteLine($"Options:");
        WriteLine("  --show-tokens: display the generated lexed tokens instead of text");

        return -1;
    }

    private int DisplayFileContents(string contents, string ext, bool showTokens = false)
    {
        var lines = contents.Split('\n');
        var syntaxHighlighter = new GenericSyntaxGenerator();

        for (int i = 0; i < lines.Length; ++i)
        {
            if (!showTokens)
            {
                var line = syntaxHighlighter.Generate(lines[i], ext);
                var lno = i + 1;
                WriteLine($"{lno} | {line}");
                continue;
            }
            else
            {
                var tokens = syntaxHighlighter.GetTokens(lines[i], ext);
                foreach (var token in tokens)
                {
                    WriteLine(token.ToString());
                }
            }
        }

        return 0;
    }
}
