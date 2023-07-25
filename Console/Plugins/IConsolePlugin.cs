
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

    public void OnLoaded(Terminal terminal);
    public void OnUnloaded(Terminal terminal);

    public bool OnUserInput(Terminal terminal, string input);
    public void OnCommandExecuted(Terminal terminal, Commands.ICommand command);

    public void OnSettingChange(ISettings settings, string settingName, object newValue);
}
