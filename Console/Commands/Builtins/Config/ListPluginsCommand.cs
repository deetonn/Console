using Console.Commands;
using Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console.Commands.Builtins.Config;

public class ListPluginsCommand : BaseBuiltinCommand
{
    public override string Name => "list_plugins";
    public override string Description => "List all loaded plugins";

    public override int Run(List<string> args, Terminal parent)
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
}
