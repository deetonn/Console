

using Console.Errors;
using Console.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Console.Formatting;

public class InlineTextFormatter : ITextFormatter
{
    public Dictionary<string, TextFormatHandler> Handlers { get; private set; }

    public InlineTextFormatter()
    {
        Handlers = [];

        AddHandler("x", (specifier, input, err) =>
        {
            if (specifier.Length == 1)
            {
                // This is invalid. There must be a number after it at LEAST.
                return err.WithMessage($"expected a number to repeat \"{input}\" by.")
                    .WithNote("example: {:../:x5} would expand to \"../../../../../\".")
                    .WithNote("it will be repeated [italic]x[/] amount of times...")
                    .Build();
            }

            if (!uint.TryParse(specifier[1..], out var count))
            {
                return err.WithMessage("invalid number to repeat the sequence by.").Build();
            }

            return string.Join("", Enumerable.Repeat(input, (int)count));
        });
    }

    public void AddHandler(string handledSpecifier, TextFormatHandler handler)
    {
        Handlers[handledSpecifier] = handler;
    }

    public CommandError? Format(string text, string specifier, out string? result)
    {
        TextFormatHandler? handler = null;

        Handlers.ForEach((key, value) =>
        {
            if (specifier.StartsWith(key))
            {
                handler = value;
            }
        });

        if (handler is null)
        {
            result = null;
            return new CommandErrorBuilder()
                .WithMessage($"no handler for the specifier \"{specifier}\"")
                .WithNote("all handled specifiers below:")
                .WithNote($"{string.Join(", ", Handlers.Keys)}")
                .Build();
        }

        var builder = new CommandErrorBuilder().WithSource($"{{:{text}:{specifier}}}");

        var parseResult = handler.Invoke(specifier, text, builder);
        result = parseResult.Formatted;
        return parseResult.Error;
    }
}
