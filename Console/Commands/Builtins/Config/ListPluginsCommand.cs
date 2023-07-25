﻿using Console.Commands;
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
        if (parent.PluginManager.Plugins.Count == 0)
        {
            WriteLine("There are no loaded plugins!");
        }

        foreach (var (id, data) in parent.PluginManager.Plugins)
        {
            WriteLine($"{data.Plugin.Name} - {data.Plugin.Description} - {data.Plugin.Author} - {id} (Active={data.Active})");
        }

        return 0;
    }
}
