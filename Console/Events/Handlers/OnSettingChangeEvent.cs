namespace Console.Events.Handlers;

using OnSettingChangeEventHandler = Func<OnSettingChangeEventArgs, bool>;

public record class OnSettingChangeEventArgs
    (string TechnicalName, string FriendlyName, object? OldValue, object? NewValue);

public class OnSettingChangeEvent : IGlobalEventHandler
{
    public OnSettingChangeEventHandler Handler { get; set; }

    public OnSettingChangeEvent(OnSettingChangeEventHandler handler)
    {
        Handler = handler;
    }

    public Event HandledEvent() => Event.OnSettingChange;

    public bool Handle(OnSettingChangeEventArgs args) => Handler(args);
}
