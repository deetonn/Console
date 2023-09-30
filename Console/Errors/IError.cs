
using System.Text;

namespace Console.Errors;

public interface IError
{
    string GetFormatted();
}

public class CommandError : IError
{
    private readonly string _built;

    public CommandError(string source, string message, List<string>? notes = null)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"[italic red]ERROR[/]: {message}");
        sb.AppendLine(" --> <repl>:1:0");
        sb.AppendLine($"1 | {source}");

        if (notes is not null)
        {
            foreach (var note in notes)
            {
                sb.AppendLine($"  = note: {note}");
            }
        }

        _built = sb.ToString();
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

    public CommandErrorBuilder WithSource(string src)
    {
        _source = src;
        return this;
    }

    public CommandErrorBuilder WithMessage(string message)
    {
        _message = message;
        return this;
    }

    public CommandErrorBuilder WithNote(string note)
    {
        _notes ??= new List<string>();
        _notes.Add(note);
        return this;
    }

    public CommandError Build() => new CommandError(_source, _message, _notes);
}