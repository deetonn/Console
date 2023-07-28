using Console;
using Console.Commands;
using Console.Plugins;
using Console.Utilitys.Configuration;
using Console.Utilitys.Options;
using Newtonsoft.Json;

namespace AutoConfig;

public class AutoConfig
{
    public bool ForceSettings { get; set; } = false;
}

public class AutoConfigDisableForceCommand : BaseBuiltinCommand
{
    public override string Name => "AutoConfig";

    public override string Description => "Interface with the auto config plugin.";

    public override DateTime? LastRunTime { get; set; }

    public override int Run(List<string> args, Terminal parent)
    {
        if (args.Count == 0)
        {
            WriteLine($"{Name}: no arguments supplied. Use `docs {Name}` for usage.");
            return -1;
        }

        var driver = parent.PluginManager.GetPlugin<AutoConfigPlugin>();

        if (driver is null)
        {
            WriteLine($"{Name}: failed to locate the {Name} plugin.");
            return -1;
        }

        var split = args[0].Split('=');

        if (split.Length != 2)
        {
            WriteLine($"{Name}: invalid option supplied.");
            return -1;
        }

        var option = split[0];
        var value = split[1];

        if (option == "--force-config")
        {
            if (!bool.TryParse(value, out bool val))
            {
                WriteLine($"{Name}: invalid value for {option} (expected true or false)");
                return -1;
            }

            driver.Options.ForceSettings = val;
        }
        else
        {
            WriteLine($"{Name}: unknown option `{option}`.");
            return -1;
        }

        return 0;
    }

    public override string DocString => $@"
This command gives an interface for the AutoConfig plugin.

This command is used to disable the force settings option in the AutoConfig plugin.

Usage:
  {Name} --force-config=false
  {Name} --force-config=true
";
}

public class AutoConfigPlugin : IConsolePlugin
{
    public string Name => "AutoConfig";

    public string Description => "Locks your configuration into a nice looking config by default.";

    public string Author => "Deeton Rushton";

    public Guid Id { get; set; } = Guid.Empty;

    public ConfigSection ConfigFolder { get; set; } = null!;

    public ConfigFile Config { get; set; } = null!;

    public AutoConfig Options { get; set; } = null!;

    public static AutoConfig Defaults => new() { ForceSettings = true };

    public void OnCommandExecuted(Terminal terminal, ICommand command)
    {
        return;
    }

    public void OnLoaded(Terminal terminal)
    {
        // TODO: Add a config file to the config folder that contains the default config.
        ConfigFolder = terminal.Config.MakeSection(terminal, Name);
        Config = ConfigFolder.MakeFile("settings");

        if (Config.WasJustCreated)
        {
            Options = Defaults;
        }
        else
        {
            Options = Config.Deserialize<AutoConfig>() ?? Defaults;
        }

        // setup the configuration to be pukka!
        var manager = terminal.Settings;

        foreach (var setting in manager.Options)
        {
            if (setting.TechnicalName == ConsoleOptions.Setting_BlockColor)
            {
                // This will be disabled.
                setting.Value = "#000000";
            }
            if (setting.TechnicalName == ConsoleOptions.Setting_MachineNameColor)
            {
                // Nice purple hue.
                setting.Value = "#9452FF";
            }
            if (setting.TechnicalName == ConsoleOptions.Setting_UserNameColor)
            {
                // Nice light green.
                setting.Value = "#73FF7A";
            }
        }

        manager.Save(terminal);
        terminal.Commands.LoadCustomCommand(new AutoConfigDisableForceCommand());
    }

    public bool OnSettingChange(Terminal terminal, ISettings settings, string settingName, object newValue)
    {
        if (Options.ForceSettings)
        {
            terminal.WriteLine($"{Name}: force block new settings is enabled, {settingName} has been blocked from changes.");
            return false;
        }

        return true;
    }

    public void OnUnloaded(Terminal terminal)
    {
        Config.WriteAll(JsonConvert.SerializeObject(Options));
    }

    public bool OnUserInput(Terminal terminal, string input)
    {
        return true;
    }
}