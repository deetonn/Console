
namespace Console.Commands.Builtins.Etc.Lexer;

public enum GenericTokenType
{
    None,
    String,
    Number,
    Keyword,
    Identifier
} 

public record class GenericToken(
    string Lexeme,
    GenericTokenType Type
    );

public class GenericLexer
{
    private List<GenericToken> tokens;
    private List<string> genericKeywords = new()
    {
        "if", "else",
        "function", "fn", "func",
        "void", "int", "long", "unsigned", "string", "char",
        "class", "struct", "record",
        "public", "private", "protected", "internal",
        "new", "delete",
        "while", "for", "let"
    };

    public GenericLexer()
    {
        tokens = new List<GenericToken>();
    }

    public List<GenericToken> Lex(string src)
    {
        if (src.Length == 0)
            return tokens;

        if (src.Last() != '\0')
        {
            src += '\0';
        }

        // very basic lexer.
        for (var i = 0; i < src.Length; ++i)
        {
            var current = src[i];
            if (current == '\0')
                break;

            string lexeme = string.Empty;

            if (current == '"' || current == '\'')
            {
                current = src[++i];
                // parse string
                                  /*"*/              /**/
                while (current != 0x22 && current != 0x27)
                {
                    lexeme += current;
                    if ((++i) == src.Length)
                        break;
                    current = src[i];
                }

                tokens.Add(new GenericToken(lexeme, GenericTokenType.String));
                continue;
            }

            if (char.IsNumber(current))
            {
                while (!char.IsNumber(current))
                {
                    lexeme += current;
                    current = src[++i];
                }

                tokens.Add(new GenericToken(lexeme, GenericTokenType.Number));
            }

            if (char.IsLetter(current))
            {
                while (true)
                {
                    lexeme += current;
                    if (!char.IsLetter(current))
                        break;
                    current = src[++i];
                }

                if (genericKeywords.Contains(lexeme.Trim().ToLower()))
                {
                    tokens.Add(new GenericToken(lexeme, GenericTokenType.Keyword));
                    continue;
                }

                tokens.Add(new GenericToken(lexeme, GenericTokenType.Identifier));
                continue;
            }

            tokens.Add(new GenericToken(current.ToString(), GenericTokenType.None));
        }

        return tokens;
    }
}
