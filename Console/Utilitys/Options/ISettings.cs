
namespace Console.Utilitys.Options;

public interface IOption
{
    /// <summary>
    /// The options technical name.
    /// </summary>
    public string TechnicalName  { get; set; }

    /// <summary>
    /// The options visual name, should be suitable for displaying
    /// on the user interface.
    /// </summary>
    public string VisualName { get; set; }

    /// <summary>
    /// The options value. This is an object as it can be anything.
    /// </summary>
    public object Value { get; set; } 
}

public interface ISettings
{
    public List<ConsoleOption> Options { get; set; }
    public string SavePath { get; }

    public void SetOption(
        string TechnicalName,
        Func<IOption, IOption> editor);

    public T? GetOptionValue<T>(string TechnicalName);
    public string? GetOptionVisualName(string TechnicalName);

    public bool OptionExists(string TechnicalName);

    public void Save();
}
