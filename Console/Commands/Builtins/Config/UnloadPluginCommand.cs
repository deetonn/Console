using Console.Errors;

namespace Console.Commands.Builtins.Config;

public class UnloadPluginCommand : BaseBuiltinCommand
{
    public override string Name => "unload_plugin";
    public override string Description => "Unload a plugin";

    public override CommandResult Run(List<string> args, IConsole parent)
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

    public override string DocString => $@"
This command will unload a plugin with a specified ID.

The commands syntax is as follows:
  {Name} <plugin_id>

To find a plugin ID, you must use the `list_plugins` command. When a plugin is loaded,
it is assigned an entirely random and unique identifier. This is used to identify the plugin
when unloading it.
";
}
