using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console.Commands.Builtins.Config;

public class UnloadPluginCommand : BaseBuiltinCommand
{
    public override string Name => "unload_plugin";
    public override string Description => "Unload a plugin";

    public override int Run(List<string> args, Terminal parent)
    {
        base.Run(args, parent);

        if (args.Count == 0)
        {
            WriteLine("Please specify a plugin to unload.");
            return -1;
        }

        var pluginId = args[0];

        if (!Guid.TryParse(pluginId, out var guid))
        {
            WriteLine("The specified plugin id is not a valid guid. To find a plugin id, use `list_plugins`.");
            return -1;
        }

        parent.PluginManager.UnloadSinglePlugin(parent, guid);
        return 0;
    }
}
