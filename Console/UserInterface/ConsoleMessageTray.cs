using Console.UserInterface.UiTypes;

namespace Console.UserInterface;

public class ConsoleMessageTray : IMessageTray
{
    public List<string> Messages { get; }

    public ConsoleMessageTray()
    {
        Messages = new List<string>();
    }

    public void AddMessage(string message)
    {
        Messages.Add(message);
    }

    public void ClearMessages()
    {
        Messages.Clear();
    }
}