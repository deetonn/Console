namespace Console.Events.Handlers;

using OnUserInputEventHandler = Func<UserInputArgs, bool>;

public record class UserInputArgs
    (string Input);

public class OnUserInputEvent : IGlobalEventHandler
{
    public Event HandledEvent() => Event.OnUserInput;

    private readonly OnUserInputEventHandler onUserInputEventHandler;

    public OnUserInputEvent(OnUserInputEventHandler onUserInputEventHandler)
    {
        this.onUserInputEventHandler = onUserInputEventHandler;
    }

    public bool Handle(UserInputArgs args)
    {
        return onUserInputEventHandler(args);
    }
}
