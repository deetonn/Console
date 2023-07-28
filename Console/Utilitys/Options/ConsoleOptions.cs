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

    public Terminal Parent { get; }

    public ConsoleOptions(string savePath, Terminal parent)
    {
        Parent = parent;
        Options = new List<ConsoleOption>();
        SavePath = savePath;

        Logger().LogInfo(this, $"Initialized the options manager. [{this}, path={savePath}]");

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
    public Color UsernameColor => GetOptionValue<Color>(Setting_UserNameColor);
    public const string Setting_MachineNameColor = "ui.color.machinename";
    public Color MachineNameColor => GetOptionValue<Color>(Setting_MachineNameColor);
    public const string Setting_WatermarkColor = "ui.color.watermark";
    public Color WatermarkColor => GetOptionValue<Color>(Setting_WatermarkColor);
    public const string Setting_TextColor = "ui.color.text";
    public Color TextColor => GetOptionValue<Color>(Setting_TextColor);

    public const string Setting_StrictMode = "execution.strictmode";
    public const bool StrictModeDefaultValue = true;

    public const string Setting_ShowBlock = "ui.options.block";
    public const string Setting_BlockColor = "ui.color.block";
    public const string Setting_DisplayWatermark = "ui.options.watermark_enabled";

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

        SetOption(Setting_ShowBlock, (opt) =>
        {
            opt.VisualName = "Show a block of color after each command to seperate outputs.";
            opt.Value = "true";
            return opt;
        });

        SetOption(Setting_BlockColor, (opt) =>
        {
            opt.VisualName = $"The color of the seperator block, only relevant if '{Setting_ShowBlock}' is enabled.";
            opt.Value = "#FD8CFF";
            return opt;
        });

        SetOption(Setting_DisplayWatermark, (opt) =>
        {
            opt.VisualName = "Display a watermark with output";
            opt.Value = "false";
            return opt;
        });

        SetOption(Setting_StrictMode, (opt) =>
        {
            opt.VisualName = "Strict mode enables errors when user-defined logic contains problems.";
            opt.Value = StrictModeDefaultValue;
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
                try
                {
                    return (T?)(object?)Terminal.MakeColorFromHexString(hexString);
                }
                catch { }
            }

            return default;
        }

        // FIXME: may cause issues elsewhere for commands
        // that use boolean values.
        if (typeof(T) == typeof(bool))
        {
            if (value is bool b)
            {
                try
                {
                    return (T?)(object)b;
                }
                catch { }
            }

            if (value is string s)
            {
                if (bool.TryParse(s, out bool res))
                    return (T?)(object)res;
            }

            return default;
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

    public bool SetOption(string TechnicalName, Func<IOption, IOption> editor)
    {
        var editor_cb = (IOption option) =>
        {
            editor(option);
            Logger().LogInfo(this, $"Set option `{option.TechnicalName}` to `{option.Value}`");
            return option;
        };

        if (!OptionExists(TechnicalName))
        {
            var option = new ConsoleOption
            {
                TechnicalName = TechnicalName
            };
            var final = editor_cb(option);
            Options.Add((ConsoleOption)final);
        }
        else
        {
            var option = Options
                .Where(x => x.TechnicalName == TechnicalName)
                .First();
            var index = Options.IndexOf(option);
            var value = (ConsoleOption)editor_cb(option);
            // Only set the new value if all plugins allow it.
            if (Parent.PluginManager.OnSettingChange(Parent, this, TechnicalName, Options[index].Value))
            {
                Options[index] = value;
                Save(Parent);
                return true;
            }

            // Nothing changed, dont save.
            return false;
        }

        Save(Parent);
        return true;
    }

    public void Save(Terminal parent)
    {
        // keep sync for safety, no way to safely
        // save between threads currently.
        var serialized = JsonConvert.SerializeObject(Options, Formatting.Indented);
        Logger().LogInfo(this, $"Saved console options with json byte count of `{serialized.Length}`");
        File.WriteAllText(Path.Combine(parent.WorkingDirectory, SavePath), serialized);
    }

    public bool RemoveOption(string TechnicalName)
    {
        if (!OptionExists(TechnicalName))
            return false;
        var match = Options.Where(x => x.TechnicalName == TechnicalName).First();
        Options.Remove(match);
        Parent.PluginManager.OnSettingChange(Parent, this, TechnicalName, null!);
        Logger().LogInfo(this, $"Remove option `{TechnicalName}`");
        return true;
    }

    override public string ToString()
    {
        return $"ConsoleOptions(Count={Options.Count})";
    }
}
