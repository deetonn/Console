
using Console.Commands;
using Console.Events;
using Console.Events.Handlers;
using Console.Utilitys.Options;
using System.Reflection;

namespace Console.Plugins;

/// <summary>
/// This interface represents a plugin loader. It can load
/// Console plugins. It is not responsible for unloading them.
/// </summary>
public interface IPluginLoader
{
    /// <summary>
    /// This function will load all plugins in the <paramref name="path"/> directory.
    /// If the path is null, the default configuration
    /// plugins directory is used. The default for this is [config]/plugins/
    /// </summary>
    /// <param name="path">The path to load these plugins from</param>
    /// <returns>A list of all the loaded plugins. If any failed to load, they are ignored.</returns>
    public Task<List<IConsolePlugin>> LoadFromPath(string path);

    /// <summary>
    /// This function will load a single plugin from the <paramref name="path"/>.
    /// </summary>
    /// <param name="path">The path to the file, must not point to a directory.</param>
    /// <returns>The loaded plugin, or null if it failed to be loaded.</returns>
    public IConsolePlugin? LoadSinglePlugin(string path);

    // Unloading will be managed by the manager, as it's a matter
    // of calling .OnUnloaded() on each plugin and removing
    // them from it's own internal storage.
}

public class PluginData
{
    public required IConsolePlugin Plugin { get; set; }
    public bool Active { get; set; } = true;
}

/// <summary>
/// This interface is responsible for managing plugins.
/// </summary>
public interface IPluginManager
{
    /// <summary>
    /// All loaded plugins.
    /// </summary>
    public IDictionary<Guid, PluginData> Plugins { get; }

    /// <summary>
    /// The plugin loader.
    /// </summary>
    public IPluginLoader Loader { get; }

    /// <summary>
    /// Load all plugins in the [config]/plugins folder.
    /// </summary>
    /// <param name="terminal">The parent terminal instance that is executing this action.</param>
    public Task LoadPlugins(IConsole terminal);

    /// <summary>
    /// Load a single plugin from <paramref name="path"/>
    /// </summary>
    /// <param name="terminal">The terminal instance executing this action.</param>
    /// <param name="path">The path to the plugin to be loaded.</param>
    public Task LoadSinglePlugin(IConsole terminal, string path);

    /// <summary>
    /// Unload all plugins, calling <see cref="IConsolePlugin.OnUnloaded(Terminal)"/>
    /// </summary>
    /// <param name="terminal">The terminal instance executing this operation.</param>
    public Task UnloadPlugins(IConsole terminal);

    /// <summary>
    /// Unload a single plugin with the name <paramref name="name"/>,
    /// then call <see cref="IConsolePlugin.OnUnloaded(Terminal)"/>"/>
    /// </summary>
    /// <param name="terminal">The parent terminal executing this action.</param>
    /// <param name="id">The identifier of the plugin to unload.</param>
    public Task UnloadSinglePlugin(IConsole terminal, Guid id);

    /// <summary>
    /// Activate a plugin. This will allow it to be called by the
    /// hook methods and allow it to function as normal.
    /// </summary>
    /// <param name="terminal">The parent terminal.</param>
    /// <param name="id">The id of the plugin to activate.</param>
    public Task<bool> ActivatePlugin(IConsole terminal, Guid id);

    /// <summary>
    /// De-activate a plugin. This will prevent it from being called
    /// by the hook methods and prevent it from functioning as normal.
    /// </summary>
    /// <param name="terminal"></param>
    /// <param name="id"></param>
    public Task<bool> DeactivatePlugin(IConsole terminal, Guid id);

    public T? GetPlugin<T>() where T : IConsolePlugin;
}

public class PluginLoader : IPluginLoader
{
    public async Task<List<IConsolePlugin>> LoadFromPath(string path)
    {
        if (!Path.Exists(path))
        {
            return Array.Empty<IConsolePlugin>().ToList();
        }

        var files = Directory.GetFiles(path, "*.dll");
        var plugins = new List<IConsolePlugin>();

        // use an async foreach to load all the commands
        // if the DLL has any files in it that inherit from
        // IConsoleCommand then load them.
        await Parallel.ForEachAsync(files, (file, stop) =>
        {
            var assembly = Assembly.LoadFrom(file);
            var types = assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IConsolePlugin)));

            foreach (var type in types)
            {
                if (Activator.CreateInstance(type) is IConsolePlugin plugin)
                {
                    plugins.Add(plugin);
                }
            }

            return ValueTask.CompletedTask;
        });

        Logger().LogInfo(this, $"Loaded {plugins.Count} plugins!");

        return plugins;
    }

    public IConsolePlugin? LoadSinglePlugin(string path)
    {
        if (!File.Exists(path))
            return null;

        var assembly = Assembly.LoadFrom(path);
        var types = assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IConsolePlugin)));

        return Activator.CreateInstance(types.First()) as IConsolePlugin;
    }
}

public class PluginManager : IPluginManager
{
    public Dictionary<Guid, PluginData> Plugins { get; }
    public IPluginLoader Loader { get; }

    IDictionary<Guid, PluginData> IPluginManager.Plugins => Plugins;

    public PluginManager(IConsole console)
    {
        Plugins = new();
        Loader = new PluginLoader();

        console.EventHandler.RegisterEvent(Event.OnUserInput, new OnUserInputEvent(x =>
        {
            var promise = OnUserInput(console, x.Input);
            return promise.Result;
        }));
        console.EventHandler.RegisterEvent(Event.OnSettingChange, new OnSettingChangeEvent(x =>
        {
            return OnSettingChange(console, console.Settings, x.TechnicalName, x.NewValue);
        }));
    }

    public Task LoadPlugins(IConsole terminal)
    {
        var plugins = Loader.LoadFromPath(Path.Combine(terminal.GetConfigPath(), "plugins")).Result;
        foreach (var plugin in plugins)
        {
            // assign each a unique Guid.
            plugin.Id = Guid.NewGuid();
            OnSinglePluginLoaded(plugin);

            if (AttemptReload(terminal, plugin))
                continue;

            Plugins.Add(plugin.Id, new PluginData { Plugin = plugin });
            plugin.OnLoaded(terminal);

            Logger().LogWarning(this, $"The `{plugin.Name}` plugin has been loaded with the id `{plugin.Id}`");
        }
        return Task.CompletedTask;
    }

    public Task LoadSinglePlugin(IConsole terminal, string path)
    {
        var plugin = Loader.LoadSinglePlugin(path);
        if (plugin is null)
        {
            terminal.Ui.DisplayLine($"failed to load plugin from path `{path}`");
            return Task.CompletedTask;
        }
        plugin.Id = Guid.NewGuid();
        OnSinglePluginLoaded(plugin);

        if (AttemptReload(terminal, plugin))
            return Task.CompletedTask;

        Plugins.Add(plugin.Id, new PluginData { Plugin = plugin });
        plugin.OnLoaded(terminal);

        Logger().LogWarning(this, $"The `{plugin.Name}` plugin has been loaded with the id `{plugin.Id}`");

        return Task.CompletedTask;
    }

    public Task UnloadPlugins(IConsole terminal)
    {
        foreach (var plugin in Plugins)
        {
            plugin.Value.Plugin.OnUnloaded(terminal);
        }

        Plugins.Clear();
        return Task.CompletedTask;
    }

    public Task UnloadSinglePlugin(IConsole terminal, Guid id)
    {
        if (!Plugins.ContainsKey(id))
            return Task.CompletedTask;

        var data = Plugins[id];
        data.Plugin.OnUnloaded(terminal);
        Plugins.Remove(id);
        return Task.CompletedTask;
    }

    public Task<bool> ActivatePlugin(IConsole terminal, Guid id)
    {
        if (!Plugins.ContainsKey(id))
            return Task.FromResult(false);

        var data = Plugins[id];
        data.Active = true;
        return Task.FromResult(true);
    }

    public Task<bool> DeactivatePlugin(IConsole terminal, Guid id)
    {
        if (!Plugins.ContainsKey(id))
            return Task.FromResult(false);

        var data = Plugins[id];
        data.Active = false;
        return Task.FromResult(true);
    }

    private Task<bool> OnUserInput(IConsole terminal, string input)
    {
        foreach (var (_, data) in Plugins)
        {
            if (!data.Active)
                continue;

            var hookWantsExit = !data.Plugin.OnUserInput(terminal, input);

            if (hookWantsExit)
                return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    private void OnCommandExecuted(IConsole terminal, ICommand command)
    {
        foreach (var (_, data) in Plugins)
        {
            if (!data.Active)
                continue;

            data.Plugin.OnCommandExecuted(terminal, command);
        }
    }

    private bool OnSettingChange(IConsole terminal, ISettings settings, string settingName, object? newValue)
    {
        foreach (var (_, data) in Plugins)
        {
            if (!data.Active)
                continue;

            if (!data.Plugin.OnSettingChange(terminal, settings, settingName, newValue))
            {
                // log so the user can locate the plugin if its a bad actor.
                Logger().LogWarning(this, $"The plugin `{data.Plugin.Name}` is blocking settings from changing.");
                return false;
            }
        }

        return true;
    }

    private bool AttemptReload(IConsole terminal, IConsolePlugin plugin)
    {
        foreach (var (key, value) in Plugins.Where(x => x.Value.Plugin.GetType() == plugin.GetType()))
        {
            // simply reload the plugin instead of creating a new instance.
            plugin.Id = key;
            Plugins[key] = new PluginData { Plugin = plugin, Active = value.Active };

            Logger().LogInfo(this, $"Reloaded plugin `{plugin.Name}` by {plugin.Author}");
            plugin.OnLoaded(terminal);
            return true;
        }

        return false;
    }

    private void OnSinglePluginLoaded(IConsolePlugin plugin)
    {
        var allPlugins = Plugins.Values.Select(p => p.Plugin).ToList();

        foreach (var otherPlugin in allPlugins.Where(x => x.Name == plugin.Name))
        {
            Logger().LogWarning(this, $"Hello {plugin.Author}! Your plugin's name appears to conflict " +
                $"with another plugin made by {otherPlugin.Author}. This is not a direct problem, but may be confusing for others " + 
                "if they have the same plugin loaded.");
        }
    }

    public T? GetPlugin<T>() where T : IConsolePlugin
    {
        return (T?)
            Plugins.Select(x => x.Value.Plugin)
                .FirstOrDefault(x => x is T);
    }
}
