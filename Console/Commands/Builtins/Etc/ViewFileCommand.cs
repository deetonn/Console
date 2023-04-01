
using Console.Commands.Builtins.Etc.Lexer;
using Console.UserInterface;
using Pastel;
using System.Drawing;
using System.IO;
using System.Text;

namespace Console.Commands.Builtins.Etc;

public interface ISyntaxGenerator
{
    string Generate(string Source);
}

public class GenericSyntaxGenerator : ISyntaxGenerator
{

    public string Generate(string Source)
    {
        var lexer = new GenericLexer();
        var tokens = lexer.Lex(Source);

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
                _ => tok.Lexeme
            };

            sb.Append(data);
        }

        return sb.ToString();
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

        if (Path.IsPathRooted(args[0]))
        {
            var path0 = Path.GetFullPath(args[0]);
            if (!File.Exists(path0))
            {
                WriteLine($"No such file '{path0}'");
                return -1;
            }
            var contents0 = File.ReadAllText(path0);
            return DisplayFileContents(contents0);
        }

        var path = Path.Combine(parent.WorkingDirectory, args[0]);
        if (!File.Exists(path))
        {
            WriteLine($"No such file '{path}'");
            return -1;
        }

        var contents = File.ReadAllText(path);
        return DisplayFileContents(contents);
    }

    private int DisplayUsage()
    {
        WriteLine($"USAGE -- {Name}");
        WriteLine($"{Name} <file-name>");

        return -1;
    }

    private int DisplayFileContents(string contents)
    {
        var lines = contents.Split('\n');
        var syntaxHighlighter = new GenericSyntaxGenerator();

        for (int i = 0; i < lines.Length; ++i)
        {
            var line = syntaxHighlighter.Generate(lines[i]);
            var lno = i + 1;
            WriteLine($"{lno} | {line}");
        }

        return 0;
    }
}
