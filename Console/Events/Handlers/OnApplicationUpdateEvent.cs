namespace Console.Events.Handlers;

using OnApplicationUpdateEventHandler = Action<OnApplicationUpdateEventArgs>;

public record class OnApplicationUpdateEventArgs
    (Web.TerminalVersionInfo NewVersion);

public class OnApplicationUpdateEvent
{
    public Event HandledEvent() => Event.OnApplicationUpdate;

    private readonly OnApplicationUpdateEventHandler onApplicationUpdateEventHandler;

    public OnApplicationUpdateEvent(OnApplicationUpdateEventHandler onApplicationUpdateEventHandler)
    {
        this.onApplicationUpdateEventHandler = onApplicationUpdateEventHandler;
    }

    public void Handle(OnApplicationUpdateEventArgs args)
    {
        onApplicationUpdateEventHandler(args);
    }
}
