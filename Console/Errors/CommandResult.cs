
namespace Console.Errors;

public class CommandResult
{
    private readonly int? _res;
    private readonly IError? _err;

    public CommandResult(int result)
    {
        _res = result;
    }
    public CommandResult(IError error)
    {
        _err = error;
    }

    public bool IsError() => _err != null;

    public int GetResult() => _res!.Value;
    public IError GetError() => _err!;

    public static implicit operator CommandResult(int value)
    {
        return new(value);
    }

    public static implicit operator CommandResult(CommandError error)
    {
        return new(error);
    }
}
