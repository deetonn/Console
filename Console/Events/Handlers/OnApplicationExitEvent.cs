
namespace Console.Events.Handlers;

using OnApplicationExitEventHandler = Action<OnApplicationExitEventArgs>;

public record class OnApplicationExitEventArgs
    (IConsole Terminal);

public class OnApplicationExitEvent : IGlobalEventHandler
{
    public Event HandledEvent() => Event.OnApplicationExit;

    private readonly OnApplicationExitEventHandler onApplicationExitEventHandler;

    public OnApplicationExitEvent(OnApplicationExitEventHandler onApplicationExitEventHandler)
    {
        this.onApplicationExitEventHandler = onApplicationExitEventHandler;
    }

    public void Handle(OnApplicationExitEventArgs args)
    {
        onApplicationExitEventHandler(args);
    }
}
