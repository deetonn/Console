

using Console.Errors;
using Console.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Console.Formatting;

public class InlineTextFormatter : ITextFormatter
{
    public Dictionary<string, TextFormatHandler> Handlers { get; private set; }
    private readonly IConsole console;

    public InlineTextFormatter(IConsole console)
    {
        Handlers = [];
        this.console = console;

        AddHandler("x", (specifier, input, err) =>
        {
            if (specifier.Length == 1)
            {
                // This is invalid. There must be a number after it at LEAST.
                return err.WithMessage($"expected a number to repeat \"{input}\" by.")
                    .WithNote($"example: {{:{input}:x5}} would expand to \"{string.Join("", Enumerable.Repeat(input, 5))}\".")
                    .WithNote("it will be repeated [italic]x[/] amount of times...")
                    .Build();
            }

            if (!uint.TryParse(specifier[1..], out var count))
            {
                return err.WithMessage("invalid number to repeat the sequence by.").Build();
            }

            return string.Join("", Enumerable.Repeat(input, (int)count));
        });
        AddHandler("trim", (specifier, input, _) =>
        {
            return input.Trim();
        });
        AddHandler("strip", (_, input, _) =>
        {
            return input.Replace(" ", "");
        });
    }

    public void AddHandler(string handledSpecifier, TextFormatHandler handler)
    {
        if (Handlers.ContainsKey(handledSpecifier))
        {
            Logger().LogWarning(this, $"The format specifier \"{handledSpecifier}\" has been overriden.");
        }

        Handlers[handledSpecifier] = handler;
    }

    public CommandError? FormatMany(string text, string[] specifiers, out string? result)
    {
        if (specifiers.Length == 0)
        {
            return Format(text, specifiers.First(), out result);
        }

        var value = text;

        foreach (var specifier in specifiers)
        {
            var res = Format(value!, specifier, out value);

            if (res is null)
                continue;

            result = null;
            return res;
        }

        result = value;
        return null;
    }

    public CommandError? Format(string text, string specifier, out string? result)
    {
        // firstly process "text" and see if it contains environment
        // variables.

        var wasError = DoNestedInlineVariables(text, out text!);

        if (wasError is not null)
        {
            result = null;
            return wasError;
        }    

        if (specifier.Contains(','))
        {
            return FormatMany(text, specifier.Split(','), out result);
        }

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

    private CommandError? DoNestedInlineVariables(string text, out string? value)
    {
        value = null;

        return new CommandErrorBuilder()
            .WithSource(console.GetLastExecutedString())
            .WithMessage("nested environment variables are not yet supported.")
            .WithNote("check [link=https://github.com/deetonn/Console/issues/22]this github[/] page for more information about this issue.")
            .Build();

        // TODO: do not remove this dead code.
        // once a general parser has been made this function will work perfectly.

        var builder = new StringBuilder();

        for (int i = 0; i < text.Length; ++i)
        {
            if (builder[i] == '{')
            {
                i--;
                var name = text.GetAllBetweenStartingAt('}', ref i);
                var start = i;

                if (!console.EnvironmentVars.TryGet(name, out var envValue))
                {
                    return new CommandErrorBuilder()
                        .WithSource(console.GetLastExecutedString())
                        .WithSpan(new ErrorSpan(start, start + name.Length))
                        .WithMessage("nested environment variable name does not exist in the environment.")
                        .WithNote($"the name was \"{name}\"")
                        .Build();
                }

                builder.Append(envValue);
                continue;
            }

            builder.Append(text[i]);
        }

        value = builder.ToString();
        return null;
    }
}
