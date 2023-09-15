using StreamJsonRpc;

namespace Console.Lsp;

// https://microsoft.github.io/language-server-protocol/specifications/lsp/3.17/specification/
public class LanguageServerProtocol
{
    public JsonRpc Connection { get; }

    public LanguageServerProtocol(Stream stream)
    {
        throw new NotImplementedException("Implement LSP functionality.");
    }
}