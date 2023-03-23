using Console.Extensions;
using Newtonsoft.Json;
using System.Drawing;

namespace Console.Utilitys.Options;

public class ConsoleOption : IOption
{
    public ConsoleOption()
    {
        TechnicalName = "option.invalid";
        VisualName = "This is option is invalid.";
        Value = "This option contains no value.";
    }
    public ConsoleOption(string technicalName, string visualName, object value)
    {
        TechnicalName = technicalName;
        VisualName = visualName;
        Value = value;
    }

    public string TechnicalName { get; set; }
    public string VisualName { get; set; }
    public object Value { get; set; }

    public T? ValueAs<T>() where T: class
    {
        return Value as T;
    }
}

public class ConsoleOptions : ISettings
{
    public List<ConsoleOption> Options { get; set; }
    public string SavePath { get; }

    public ConsoleOptions(string savePath)
    {
        Options = new List<ConsoleOption>();
        SavePath = savePath;

        // load options or load defaults

        if (!File.Exists(savePath))
        {
            LoadDefaultOptions();
        }
        else
        {
            var fileContents = File.ReadAllText(savePath);
            Options = JsonConvert.DeserializeObject<List<ConsoleOption>>(fileContents)!;
        }
    }

    private void LoadDefaultOptions()
    {
        SetOption("colors.username", (thing) =>
        {
            thing.VisualName = "The color of the username section";
            thing.Value = Color.Red.ToHexString();
            return thing;
        });

        SetOption("colors.machinename", (thing) =>
        {
            thing.VisualName = "The color of the machine name section";
            thing.Value = Color.Yellow.ToHexString();
            return thing;
        });
    }

    public T? GetOptionValue<T>(string TechnicalName)
    {
        if (!OptionExists(TechnicalName))
            return default;

        return (T?)Options
            .Where(x => x.TechnicalName == TechnicalName)
            .FirstOrDefault()
            ?.Value;
    }

    public string? GetOptionVisualName(string TechnicalName)
    {
        if (!OptionExists(TechnicalName))
            return default;

        return Options
            .Where(x => x.TechnicalName == TechnicalName)
            .FirstOrDefault()
            ?.VisualName;
    }

    public bool OptionExists(string TechnicalName)
    {
        return Options.Any(x => x.TechnicalName == TechnicalName);
    }

    public void SetOption(string TechnicalName, Func<IOption, IOption> editor)
    {
        if (!OptionExists(TechnicalName))
        {
            var option = new ConsoleOption
            {
                TechnicalName = TechnicalName
            };
            var final = editor(option);
            Options.Add((ConsoleOption)final);
        }
        else
        {
            var option = Options
                .Where(x => x.TechnicalName == TechnicalName)
                .First();
            var index = Options.IndexOf(option);
            Options[index] = (ConsoleOption)editor(option);
        }

        Save();
    }

    public void Save()
    {
        // keep sync for safety, no way to safely
        // save between threads currently.
        var serialized = JsonConvert.SerializeObject(Options);
        File.WriteAllText(SavePath, serialized);
    }
}
