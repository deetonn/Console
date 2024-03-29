﻿using Console.Errors;

namespace Console.Commands.Builtins.Config;

public class ListPluginsCommand : BaseBuiltinCommand
{
    public override string Name => "list_plugins";
    public override string Description => "List all loaded plugins";

    public override CommandResult Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);
        var count = parent.PluginManager.Plugins.Count;

        if (count == 0)
        {
            WriteLine("There are no loaded plugins!");
            return 0;
        }

        WriteLine($"There is {count} plugins loaded.\n");

        foreach (var (id, data) in parent.PluginManager.Plugins)
        {
            WriteLine($"{data.Plugin.Name} - {data.Plugin.Description} - {data.Plugin.Author} - {id} (Active={data.Active})");
        }

        return 0;
    }

    public override string DocString => $@"
This command will list all loaded plugins.

If there are no plugins loaded, it will say so.
";
}
