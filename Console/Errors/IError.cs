
using System.Text;

namespace Console.Errors;

public interface IError
{
    string GetFormatted();
}

public record class ErrorSpan(int Start, int End);

public class CommandError : IError
{
    private readonly string _built;

    public CommandError(string source, string message, List<string>? notes = null, ErrorSpan? span = null)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"[italic red bold]ERROR[/]: {message}");
        sb.AppendLine(" --> <repl>:1:0");
        sb.AppendLine($"1 | {source}");

        if (span is not null)
        {
            int start = 4 + span.Start;
            sb.AppendLine($"[red]{new string(' ', start)}{new string('^', span.Start + span.End)}[/]");
        }

        if (notes is not null)
        {
            foreach (var note in notes)
            {
                sb.AppendLine($"  = note: {note}");
            }
        }

        _built = sb.ToString();

        Logger().LogError(this, " ---------------- ");
        Logger().LogError(this, $"An error occured:");
        Logger().LogError(this, $"--  {message}");
        Logger().LogError(this, $"--  {notes?.Count ?? 0} notes attached.");
        Logger().LogError(this, $"--    at \"{source}\"");
        Logger().LogError(this, " ---------------- ");
    }

    public string GetFormatted()
    {
        return _built;
    }
}

public class CommandErrorBuilder
{
    private string _source;
    private string _message;
    private List<string>? _notes;
    private ErrorSpan? _span;

    public CommandErrorBuilder WithSource(string src)
    {
        _source = src;
        return this;
    }

    public CommandErrorBuilder WithSpan(ErrorSpan span)
    {
        _span = span;
        return this;
    }

    public CommandErrorBuilder WithMessage(string message)
    {
        _message = message;
        return this;
    }

    public CommandErrorBuilder WithNote(string note)
    {
        _notes ??= [];
        _notes.Add(note);
        return this;
    }

    public CommandError Todo(string message)
        => WithMessage(message)
            .WithNote("This command is not yet properly implemented.")
            .Build();

    public CommandError Build() => new(_source, _message, _notes);
}