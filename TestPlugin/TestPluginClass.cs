using System;
using System.Transactions;
using Console.Commands;
using Console.Utilitys.Options;

namespace Console;

public class TestCommand : BaseBuiltinCommand
{
    public override string Name => "test";
    public override string Description => "This command exists for testing purposes";

    public override int Run(List<string> args, Terminal parent)
    {
        base.Run(args, parent);

        WriteLine("This is from the externally loaded test command!");

        return 0;
    }
}

public class ExamplePlugin : Plugins.IConsolePlugin
{
    // Doesn't have to by `_` seperated.
    public string Name => "my_plugin_name";

    public string Description => "My description";

    public string Author => "Your name";

    public Guid Id { get; set; } = Guid.Empty; // This is set by the plugin manager.

    public void OnCommandExecuted(Terminal terminal, ICommand command)
    {

    }

    public void OnLoaded(Terminal terminal)
    {
    }

    public bool OnSettingChange(Terminal terminal, ISettings settings, string settingName, object newValue)
    {
        return true;
    }

    public void OnUnloaded(Terminal terminal)
    {
    }

    public bool OnUserInput(Terminal terminal, string input)
    {
        return true;
    }
}