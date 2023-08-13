
namespace Console.Events.Handlers;

using OnApplicationStartEventHandler = Action<OnApplicationStartEventArgs>;

public record class OnApplicationStartEventArgs
    (IConsole Terminal);

public class OnApplicationStartEvent : IGlobalEventHandler
{
    public Event HandledEvent() => Event.OnApplicationStart;

    private readonly OnApplicationStartEventHandler onApplicationStartEventHandler;

    public OnApplicationStartEvent(OnApplicationStartEventHandler onApplicationStartEventHandler)
    {
        this.onApplicationStartEventHandler = onApplicationStartEventHandler;
    }

    public void Handle(OnApplicationStartEventArgs args)
    {
        onApplicationStartEventHandler(args);
    }
}
