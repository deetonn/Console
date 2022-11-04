namespace Console.UserInterface.UiTypes;

public interface IMessageTray
{
    public List<string> Messages { get; }

    public void AddMessage(string message);

    public void ClearMessages();
}