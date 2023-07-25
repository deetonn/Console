using System;
using Console;
using Console.Commands;
using Console.Utilitys.Options;

namespace TestPlugin;

public class TestPluginClass : Console.Plugins.IConsolePlugin
{
    public string Name => "test plugin";

    public string Description => "This is a test";

    public string Author => "Deeton Lee Rushton";

    public Guid Id { get; set; } = Guid.Empty;

    public void OnCommandExecuted(Terminal terminal, ICommand command)
    {
        terminal.WriteLine($"[Test] The command `{command.Name}` has been ran!");
    }

    public void OnLoaded(Terminal terminal)
    {
        terminal.WriteLine("[test] Loaded!");
    }

    public void OnSettingChange(ISettings settings, string settingName, object newValue)
    {
        // nothing here for now
    }

    public void OnUnloaded(Terminal terminal)
    {
        terminal.WriteLine("[test] I am going now!");
    }

    public bool OnUserInput(Terminal terminal, string input)
    {
        return true;
    }
}