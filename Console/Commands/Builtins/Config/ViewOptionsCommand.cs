﻿
using Console.Errors;
using Console.Utilitys.Options;

namespace Console.Commands.Builtins.Config;

public class ViewOptionsCommand : BaseBuiltinCommand
{
    public override string Name => "optview";
    public override string Description => "View the configuration settings";
    public override DateTime? LastRunTime { get; set; } = null;
    public override CommandResult Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        foreach (var option in parent.Settings.Options)
        {
            WriteLine($"[[{option.TechnicalName}]]: {option.VisualName} (Value: {option.Value})");
        }

        var savePath = parent.GetConfigPath();

        WriteLine($"\nAll options are saved inside the path ` {savePath} `");

        return 0;
    }

    public void OutputOption(ConsoleOption option)
    {
        WriteLine("");
    }

    public override string DocString => $@"
This command will output all configuration options.

It will also tell you where the options are saved.

The options are displayed like this:
    [option-name]: Visual name (Value: option-value)

      option-name: The internal name, something like org.plugin.setting
      visual-name: The name displayed to a user, something like Plugin Setting
      option-value: The value of the option, something like true or #FF0000
";
}
