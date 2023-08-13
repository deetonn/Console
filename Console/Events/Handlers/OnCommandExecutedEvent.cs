
using Console.Commands;

namespace Console.Events.Handlers;

using OnCommandExecutedEventHandler = Action<OnCommandExecutedEventArgs>;

public record class OnCommandExecutedEventArgs
    (ICommand Command);

public class OnCommandExecutedEvent : IGlobalEventHandler
{
    public Event HandledEvent() => Event.OnCommandExecuted;

    private readonly OnCommandExecutedEventHandler onCommandExecutedEventHandler;

    public OnCommandExecutedEvent(OnCommandExecutedEventHandler onCommandExecutedEventHandler)
    {
        this.onCommandExecutedEventHandler = onCommandExecutedEventHandler;
    }

    public void Handle(OnCommandExecutedEventArgs args)
    {
        onCommandExecutedEventHandler(args);
    }
}
