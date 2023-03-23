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

    public const string Setting_UserNameColor = "ui.color.username";
    public const string Setting_MachineNameColor = "ui.color.machinename";
    public const string Setting_WatermarkColor = "ui.color.watermark";
    public const string Setting_TextColor = "ui.color.text";


    private void LoadDefaultOptions()
    {
        SetOption(Setting_UserNameColor, (thing) =>
        {
            thing.VisualName = "The color of the username section";
            thing.Value = Color.Red.ToHexString();
            return thing;
        });

        SetOption(Setting_MachineNameColor, (thing) =>
        {
            thing.VisualName = "The color of the machine name section";
            thing.Value = Color.Yellow.ToHexString();
            return thing;
        });

        SetOption(Setting_WatermarkColor, (opt) =>
        {
            opt.Value = Color.Gray.ToHexString();
            opt.VisualName = "Terminals Watermark color.";
            return opt;
        });

        SetOption(Setting_TextColor, (opt) =>
        {
            opt.Value = Color.White.ToHexString();
            opt.VisualName = "The default color of text in Terminal";
            return opt;
        });
    }

    public T? GetOptionValue<T>(string TechnicalName)
    {
        if (!OptionExists(TechnicalName))
            return default;

        var value = Options
            .Where(x => x.TechnicalName == TechnicalName)
            .FirstOrDefault()
            ?.Value;

        if (typeof(T) == typeof(Color))
        {
            if (value is string hexString)
            {
                // assume its a hex string if they want it
                // as a color.
                return (T?)(object?)Terminal.MakeColorFromHexString(hexString);
            }
        }

        return (T?)value;
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
        var serialized = JsonConvert.SerializeObject(Options, Formatting.Indented);
        File.WriteAllText(SavePath, serialized);
    }
}
