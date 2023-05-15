using Runtime.Lexer;
using System.Text;

namespace Console.Commands.Builtins.Etc.Lexer;

public class DeeLHighlighter : ISyntaxGenerator
{
    public string Generate(string Source, string ext)
    {
        List<Token> tokens;

        try
        {
            tokens = new DLexer(Source).Lex();
        }
        catch
        {
            return Source;
        }

        var sb = new StringBuilder();
        var lineCount = Source.Split('\n').Length;

        for (int i = 0; i < lineCount; i++) { }
        throw new NotImplementedException();
    }



    public List<GenericToken> GetTokens(string Source, string ext)
    {
        throw new NotImplementedException();
    }
}
