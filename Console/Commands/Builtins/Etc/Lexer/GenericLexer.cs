
namespace Console.Commands.Builtins.Etc.Lexer;

public enum GenericTokenType
{
    None,
    String,
    Number,
    Keyword,
    Identifier,
    FunctionCall
} 

public record class GenericToken(
    string Lexeme,
    GenericTokenType Type
    )
{
    public override string ToString()
    {
        return $"{Type}(Lexeme: '{Lexeme}')";
    }
}

public class GenericLexer
{
    private readonly List<GenericToken> tokens;
    private readonly List<string> genericKeywords = new()
    {
        "if", "else",
        "function", "fn", "func",
        "void", "int", "long", "unsigned", "string", "char",
        "class", "struct", "record",
        "public", "private", "protected", "internal",
        "new", "delete", "from", "import",
        "while", "for", "let", "const", "volatile", "def", "elif",
        "return",
        // C++ specific shit
        "template", "typename", "noexcept", "auto",
        "constexpr", "throw"
    };
    private readonly List<string> codeFileTypes = new()
    {
        ".cpp", ".c", ".cc", ".hpp", ".h", 
        ".cs", 
        ".rs", 
        ".js", ".py", ".ts", 
        ".dl"
    };

    private bool _isCodeFile;

    public GenericLexer(string ext)
    {
        _isCodeFile = codeFileTypes.Contains(ext);
        tokens = new List<GenericToken>();
    }

    public List<GenericToken> Lex(string src)
    {
        if (!_isCodeFile)
            return new List<GenericToken> { new GenericToken(src, GenericTokenType.None) };

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
                                  /*"*/              /*'*/
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
                    current = src[++i];

                    if (current != '_' && !char.IsLetter(current))
                    {
                        --i;
                        break;
                    }
                }

                if (genericKeywords.Contains(lexeme.Trim()))
                {
                    tokens.Add(new GenericToken(lexeme, GenericTokenType.Keyword));
                    continue;
                }

                if (Peek(src, i) == '(')
                {
                    tokens.Add(new GenericToken(lexeme, GenericTokenType.FunctionCall));
                    continue;
                }

                tokens.Add(new GenericToken(lexeme, GenericTokenType.Identifier));
                continue;
            }

            tokens.Add(new GenericToken(current.ToString(), GenericTokenType.None));
        }

        return tokens;
    }

    public char Peek(string s, int i)
    {
        var res = i + 1;
        if (res > s.Length)
            return '\0';
        return s[res];
    }
}
