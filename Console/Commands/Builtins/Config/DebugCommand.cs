using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console.Commands.Builtins.Etc;
using Console.Errors;
using Console.Extensions;

namespace Console.Commands.Builtins.Config;

public class DebugCommand : BaseBuiltinCommand
{
    public override string Name => "debug";
    public override string Description => "Provides an interface to view Console's internal values.";

    private static readonly Dictionary<string, Func<IConsole, string>> ValueViewers = [];

    public override void OnInit(IConsole parent)
    {
        base.OnInit(parent);

        ValueViewers.Add("BufferWidth", ctx =>
        {
            return $"{ctx.Ui.BufferWidth}";
        });

        ValueViewers.Add("BufferHeight", ctx =>
        {
            return $"{ctx.Ui.BufferHeight}";
        });

        ValueViewers.Add("CommandCount", ctx =>
        {
            var commands = ctx.Commands.Elements;
            int builtin = 0, external = 0, alias = 0;

            foreach (var item in commands)
            {
                if (item is BaseBuiltinCommand)
                    builtin++;
                if (item is PathFileCommand)
                    external++;
                if (item is AliasCommand)
                    alias++;
            }

            return $"{commands.Count} ({builtin} builtin, {external} external, {alias} aliases)";
        });

        ValueViewers.Add("PluginCount", ctx =>
        {
            var plugins = ctx.PluginManager.Plugins;
            return $"{plugins.Count}";
        });

        ValueViewers.Add("ConfigPath", ctx =>
        {
            return $"{ctx.GetConfigPath()}";
        });

        ValueViewers.Add("UiKind", ctx =>
        {
            return ctx.Ui.GetType().Name;
        });

        ValueViewers.Add("IsAdmin", ctx =>
        {
            return ctx.IsAdministrator().ToString();
        });

        ValueViewers.Add("ExecutablePath", ctx =>
        {
            return ctx.GetExecutableLocation();
        });
    }

    public override CommandResult Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        if (args.Count == 0)
        {
            foreach (var (key, getter) in ValueViewers)
            {
                WriteLine($"{key}: {getter(parent)}");
            }

            return 0;
        }

        var debugItem = args[0];

        if (!ValueViewers.TryGetValue(debugItem, out Func<IConsole, string>? value))
        {
            return Error()
                .WithMessage($"The key `{debugItem}` is unrecognized.")
                .WithNote($"Supported keys: {string.Join(", ", ValueViewers.Keys)}")
                .Build();
        }

        var debugValue = value(parent);
        WriteLine($"{debugItem}: {debugValue}");

        return 0;
    }
}
