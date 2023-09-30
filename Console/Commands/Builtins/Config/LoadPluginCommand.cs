
using Console.Errors;

namespace Console.Commands.Builtins.Config;

public class LoadPluginCommand : BaseBuiltinCommand
{
    public override string Name => "load_plugin";

    public override string Description => "Load a plugin.";

    public override CommandResult Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        if (args.Count == 0)
        {
            return Error()
                .WithMessage("You must specify a path to a valid .DLL")
                .WithNote("Read[link=https://github.com/deetonn/Console/wiki/Plugins] here[/] for more information.")
                .Build();
        }

        var path = args[0];
        if (!File.Exists(path))
        {
            return Error()
                .WithMessage("The specified file does not exist.")
                .WithNote($"The file in question: {path}")
                .Build();
        }

        parent.PluginManager.LoadSinglePlugin(parent, path);
        return 0;
    }

    public override string DocString => $@"
This command will load a plugin from an absolute path.

The plugin documentation can be found at the projects offical github page.

Example:
    load_plugin C:\Users\user\Documents\plugin.dll

NOTE: This command can reload the same plugin. This is useful for debugging.
";
}
