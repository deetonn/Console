﻿
namespace Console.Commands.Builtins.Config;

public class LoadPluginCommand : BaseBuiltinCommand
{
    public override string Name => "load_plugin";

    public override string Description => "Load a plugin.";

    public override int Run(List<string> args, Terminal parent)
    {
        base.Run(args, parent);

        if (args.Count == 0)
        {
            WriteLine("Please specify a patch to a plugin to load.");
            return -1;
        }

        var path = args[0];
        if (!File.Exists(path))
        {
            WriteLine("The specified file does not exist.");
            return -1;
        }

        parent.PluginManager.LoadSinglePlugin(parent, path);
        return 0;
    }
}