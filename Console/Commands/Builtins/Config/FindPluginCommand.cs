using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console.Commands.Builtins.Config;

public class FindPluginCommand : BaseBuiltinCommand
{
    public override string Name => "find_plugin";
    public override string Description => "find a plugin by name.";

    public override int Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        if (args.Count != 1)
        {
            WriteError("expected one argument: \"plugin_name\"");
            return -1;
        }

        foreach (var plugin in parent.PluginManager.Plugins)
        {
            var name = plugin.Value.Plugin.Name;
            if (name == args[0])
            {
                parent.EnvironmentVars.AppendCommandOutput(plugin.Value.Plugin.Id.ToString());
                WriteLine($"{name} -- {plugin.Value.Plugin.Id}");
                return 0;
            }
        }

        WriteError("no plugin with that name exists.");
        return -1;
    }

    public override string DocString => $@"
This command will try to find a plugin base on its name.

If the plugin exists, its name and id are displayed. The environment variable
'$' will be set to the plugin id.

This is useful for automation when unloading plugins.
";
}
