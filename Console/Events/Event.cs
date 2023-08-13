namespace Console.Events;

/// <summary>
/// This type is a stand-in interface that is very general.
/// It is expected to be casted into something else.
/// </summary>
public interface IGlobalEventHandler
{
    public Event HandledEvent();
}

public enum Event
{
    /// <summary>
    /// When the user types something, it will go through here.
    /// </summary>
    OnUserInput,         // Handlers/OnUserInputEvent.cs
    OnSettingChange,     // Handlers/OnSettingChangeEvent.cs
    OnApplicationStart,  // Handlers/OnApplicationStartEvent.cs
    OnApplicationExit,   // Handlers/OnApplicationExitEvent.cs
    OnApplicationUpdate, // Handlers/OnApplicationUpdateEvent.cs
    OnCommandExecuted,   // Handlers/OnCommandExecutedEvent.cs
}
