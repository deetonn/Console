
using Console.Errors;
using System.Diagnostics.CodeAnalysis;

// using TextFormatResult = (string? FormattedValue, CommandError? Error);

namespace Console.Formatting;

public struct TextFormatResult
{
    public CommandError? Error;
    public string? Formatted;

    public static implicit operator TextFormatResult(CommandError error)
    {
        return new TextFormatResult
        {
            Error = error,
            Formatted = null
        };
    }

    public static implicit operator TextFormatResult(string fmt)
    {
        return new TextFormatResult
        {
            Error = null,
            Formatted = fmt
        };
    }
}

public delegate TextFormatResult TextFormatHandler(string fullSpecifier, string userInput, CommandErrorBuilder errBuilder);

public interface ITextFormatter
{
    public Dictionary<string, TextFormatHandler> Handlers { get; }

    /// <summary>
    /// This will add a custom format specifier.
    /// </summary>
    /// <param name="handledSpecifier">What does the specifier have to start with to match your format.</param>
    /// <param name="handler">The function that will handle it.</param>
    public void AddHandler(string handledSpecifier, TextFormatHandler handler);

    // NOTE: as of now, there is no remove method. It doesn't make sense that they
    // would be removed?

    /// <summary>
    /// Attempt to format the <paramref name="text"/> with the <paramref name="specifier"/> 
    /// and put the result into the out variable <paramref name="result"/>
    /// </summary>
    /// <param name="text">The text the user wants to format</param>
    /// <param name="specifier">The format specifier</param>
    /// <param name="result">The result</param>
    /// <returns>true if <paramref name="result"/> contains the formatted string, otherwise false.</returns>
    public CommandError? Format(string text, string specifier, out string? result);
}
