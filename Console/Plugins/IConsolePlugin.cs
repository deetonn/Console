
using Console.Commands;
using Console.Utilitys.Options;
using System.Windows.Input;

namespace Console.Plugins;

public interface IConsolePlugin
{
    public string Name { get; }
    public string Description { get; }
    public string Author { get; }
    public Guid Id { get; set; }

    public void OnLoaded(IConsole terminal);
    public void OnUnloaded(IConsole terminal);

    public bool OnUserInput(IConsole terminal, string input);
    public void OnCommandExecuted(IConsole terminal, Commands.ICommand command);

    public bool OnSettingChange(IConsole terminal, ISettings settings, string settingName, object newValue);
}
