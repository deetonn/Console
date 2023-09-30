namespace Console.Utilitys;

public class Optional<T>
{
    private T? Value { get; set; } = default;

    public Optional() { }
    public Optional(T value)
    {
        Value = value;
    }

    public bool HasValue() => Value is not null;
    public T Unwrap() => Value!;

    public static implicit operator Optional<T>(T value)
    {
        return new Optional<T>(value);
    }

}
