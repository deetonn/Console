using Console.Events.Handlers;
using Console.Utilitys;

namespace Console.Events;

public class EventHandlerCollection
{
    public IDictionary<Event, IList<IGlobalEventHandler>> Handlers { get; set; } = new Dictionary<Event, IList<IGlobalEventHandler>>()
    {
        [Event.OnApplicationStart] = new List<IGlobalEventHandler>(),
        [Event.OnApplicationExit] = new List<IGlobalEventHandler>(),
        [Event.OnUserInput] = new List<IGlobalEventHandler>(),
        [Event.OnSettingChange] = new List<IGlobalEventHandler>(),
        [Event.OnCommandExecuted] = new List<IGlobalEventHandler>(),
        [Event.OnApplicationUpdate] = new ThrowNotImplementedList<IGlobalEventHandler>()
    };

    public void Add(Event @event, IGlobalEventHandler handler)
    {
        if (Handlers.ContainsKey(@event))
        {
            Handlers[@event].Add(handler);
        }
        else
        {
            Handlers.Add(@event, new List<IGlobalEventHandler>() { handler });
        }
    }
    public void Clear(Event @event)
    {
        if (Handlers.ContainsKey(@event))
        {
            Handlers[@event].Clear();
        }
    }

    public IList<IGlobalEventHandler> GetFor(Event @event)
    {
        if (!Handlers.ContainsKey(@event))
        {
            throw new KeyNotFoundException($"The event `{@event}` has not been registered.");
        }

        return Handlers[@event];
    }
}

public class EventStats
{
    public uint TotalEvents { get; set; } = 0;
    public uint TotalHandledEvents { get; set; } = 0;
    public uint TotalUnhandledEvents { get; set; } = 0;

    public void IncrementTotalHandledEvents()
    {
        TotalHandledEvents++; TotalEvents++;
    }
    public void IncrementTotalUnhandledEvents()
    {
        TotalUnhandledEvents++;
        TotalEvents++;
    }
}

public class CancellationToken
{
    public bool IsCancellableTask { get; init; } = false;
    public bool IsCancelled { get; set; } = false;

    public static CancellationToken CreateCancellableTask(bool result)
    {
        return new CancellationToken() { IsCancellableTask = true };
    }

    public static CancellationToken CreateNonCancellableTask()
    {
        return new CancellationToken() { IsCancellableTask = false };
    }
}

public interface IEventHandler
{
    public EventHandlerCollection EventHandlers { get; set; }
    public EventStats Stats { get; }

    public void RegisterEvent(Event @event, IGlobalEventHandler handler)
    {
        EventHandlers.Add(@event, handler);
    }

    public CancellationToken HandleEvent(Event @event, object? args)
    {
        switch (@event)
        {
            case Event.OnUserInput:
                bool houiResult = HandleOnUserInput((UserInputArgs?)args);
                Stats.IncrementTotalHandledEvents();
                return CancellationToken.CreateCancellableTask(houiResult);
            case Event.OnSettingChange:
                bool hoscResult = HandleOnSettingChange((OnSettingChangeEventArgs?)args);
                Stats.IncrementTotalHandledEvents();
                return CancellationToken.CreateCancellableTask(hoscResult);
            case Event.OnApplicationStart:
                HandleOnApplicationStart((OnApplicationStartEventArgs?)args);
                Stats.IncrementTotalHandledEvents();
                return CancellationToken.CreateNonCancellableTask();
            case Event.OnApplicationExit:
                HandleOnApplicationExit((OnApplicationExitEventArgs?)args);
                Stats.IncrementTotalHandledEvents();
                return CancellationToken.CreateNonCancellableTask();
            case Event.OnApplicationUpdate:
                HandleOnApplicationUpdate((OnApplicationUpdateEventArgs?)args);
                Stats.IncrementTotalHandledEvents();
                return CancellationToken.CreateNonCancellableTask();
            default:
                Logger().LogWarning(this, $"Unhandled event type: {@event}");
                Stats.IncrementTotalUnhandledEvents();
                return CancellationToken.CreateNonCancellableTask();
        }
    }

    bool HandleOnSettingChange(OnSettingChangeEventArgs? args);
    bool HandleOnUserInput(UserInputArgs? args);

    void HandleOnCommandExecuted(OnCommandExecutedEventArgs? args);
    void HandleOnApplicationStart(OnApplicationStartEventArgs? args);
    void HandleOnApplicationExit(OnApplicationExitEventArgs? args);
    void HandleOnApplicationUpdate(OnApplicationUpdateEventArgs? args);
}

public class GlobalEventHandler : IEventHandler
{
    public EventHandlerCollection EventHandlers { get; set; }

    public GlobalEventHandler()
    {
        EventHandlers = new EventHandlerCollection();
    }

    public EventStats Stats { get; } = new EventStats();

    public bool HandleOnSettingChange(OnSettingChangeEventArgs? args)
    {
        if (args is null)
        {
            Logger().LogWarning(this, "OnSettingChangeEventArgs is null.");
            return false;
        }

        var handlers = EventHandlers.GetFor(Event.OnSettingChange);

        foreach (IGlobalEventHandler handler in handlers)
        {
            if (handler is OnSettingChangeEvent h)
            {
                if (!h.Handle(args))
                {
                    Logger().LogWarning(this, $"Handler `{h}` returned false.");
                    return false;
                }
            }
            else
            {
                Logger().LogWarning(this, $"Handler `{handler}` is not of type `OnSettingChangeEvent`.");
                return false;
            }
        }

        return true;
    }
    public bool HandleOnUserInput(UserInputArgs? args)
    {
        if (args is null)
        {
            Logger().LogWarning(this, "UserInputArgs is null.");
            return false;
        }

        var handlers = EventHandlers.GetFor(Event.OnUserInput);

        foreach (IGlobalEventHandler handler in handlers)
        {
            if (handler is OnUserInputEvent h)
            {
                if (!h.Handle(args))
                {
                    Logger().LogWarning(this, $"Handler `{h}` returned false.");
                    return false;
                }
            }
            else
            {
                Logger().LogWarning(this, $"Handler `{handler}` is not of type `OnUserInputEvent`.");
                return false;
            }
        }

        return true;
    }

    public void HandleOnCommandExecuted(OnCommandExecutedEventArgs? args)
    {
        if (args is null)
        {
            Logger().LogWarning(this, "OnCommandExecutedEventArgs is null.");
            return;
        }

        var handlers = EventHandlers.GetFor(Event.OnCommandExecuted);

        foreach (IGlobalEventHandler handler in handlers)
        {
            if (handler is OnCommandExecutedEvent h)
            {
                h.Handle(args);
            }
            else
            {
                Logger().LogWarning(this, $"Handler `{handler}` is not of type `OnCommandExecutedEvent`.");
            }
        }
    }
    public void HandleOnApplicationStart(OnApplicationStartEventArgs? args)
    {
        if (args is null)
        {
            Logger().LogWarning(this, "OnApplicationStartEventArgs is null.");
            return;
        }

        var handlers = EventHandlers.GetFor(Event.OnApplicationStart);

        foreach (IGlobalEventHandler handler in handlers)
        {
            if (handler is OnApplicationStartEvent h)
            {
                h.Handle(args);
            }
            else
            {
                Logger().LogWarning(this, $"Handler `{handler}` is not of type `OnApplicationStartEvent`.");
            }
        }
    }
    public void HandleOnApplicationExit(OnApplicationExitEventArgs? args)
    {
        if (args is null)
        {
            Logger().LogWarning(this, "OnApplicationExitEventArgs is null.");
            return;
        }

        var handlers = EventHandlers.GetFor(Event.OnApplicationExit);

        foreach (IGlobalEventHandler handler in handlers)
        {
            if (handler is OnApplicationExitEvent h)
            {
                h.Handle(args);
            }
            else
            {
                Logger().LogWarning(this, $"Handler `{handler}` is not of type `OnApplicationExitEvent`.");
            }
        }
    }
    public void HandleOnApplicationUpdate(OnApplicationUpdateEventArgs? args)
    {
        if (args is null)
        {
            Logger().LogWarning(this, "OnApplicationUpdateEventArgs is null.");
            return;
        }

        var handlers = EventHandlers.GetFor(Event.OnApplicationUpdate);

        foreach (IGlobalEventHandler handler in handlers)
        {
            if (handler is OnApplicationUpdateEvent h)
            {
                h.Handle(args);
            }
            else
            {
                Logger().LogWarning(this, $"Handler `{handler}` is not of type `OnApplicationUpdateEvent`.");
            }
        }
    }

    // use default implementations until something specific
    // comes up.
}
