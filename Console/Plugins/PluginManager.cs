﻿
using Console.Commands;
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
    public Task LoadPlugins(Terminal terminal);

    /// <summary>
    /// Load a single plugin from <paramref name="path"/>
    /// </summary>
    /// <param name="terminal">The terminal instance executing this action.</param>
    /// <param name="path">The path to the plugin to be loaded.</param>
    public Task LoadSinglePlugin(Terminal terminal, string path);

    /// <summary>
    /// Unload all plugins, calling <see cref="IConsolePlugin.OnUnloaded(Terminal)"/>
    /// </summary>
    /// <param name="terminal">The terminal instance executing this operation.</param>
    public Task UnloadPlugins(Terminal terminal);

    /// <summary>
    /// Unload a single plugin with the name <paramref name="name"/>,
    /// then call <see cref="IConsolePlugin.OnUnloaded(Terminal)"/>"/>
    /// </summary>
    /// <param name="terminal">The parent terminal executing this action.</param>
    /// <param name="id">The identifier of the plugin to unload.</param>
    public Task UnloadSinglePlugin(Terminal terminal, Guid id);

    /// <summary>
    /// Activate a plugin. This will allow it to be called by the
    /// hook methods and allow it to function as normal.
    /// </summary>
    /// <param name="terminal">The parent terminal.</param>
    /// <param name="id">The id of the plugin to activate.</param>
    public Task<bool> ActivatePlugin(Terminal terminal, Guid id);

    /// <summary>
    /// De-activate a plugin. This will prevent it from being called
    /// by the hook methods and prevent it from functioning as normal.
    /// </summary>
    /// <param name="terminal"></param>
    /// <param name="id"></param>
    public Task<bool> DeactivatePlugin(Terminal terminal, Guid id);

    /// <summary>
    /// Process this hook, calling all plugins
    /// <see cref="IConsolePlugin.OnUserInput(Terminal, string)"/> method.
    /// NOTE: This hook is called before the input is registered.
    /// If your hook returns false, any hooks after it will not be called.
    /// </summary>
    /// <param name="terminal">The parent terminal</param>
    /// <param name="input">The user input</param>
    /// <returns>True if the hook allows the input, otherwise false.</returns>
    public Task<bool> OnUserInput(Terminal terminal, string input);

    /// <summary>
    /// Process the hook, calling all plugins
    /// <see cref="IConsolePlugin.OnCommandExecuted(Terminal, Commands.ICommand)"/> method.
    /// NOTE: This hook is called after the command has been executed.
    /// </summary>
    /// <param name="terminal">The parent terminal this operation is being executed on.</param>
    /// <param name="command">The command that has been executed.</param>
    public void OnCommandExecuted(Terminal terminal, Commands.ICommand command);

    /// <summary>
    /// Process the hook, calling the 
    /// <see cref="IConsolePlugin.OnSettingChange(ISettings, string, object)"/> method.
    /// </summary>
    /// <param name="settings">The settings instance</param>
    /// <param name="settingName">The setting name.</param>
    /// <param name="newValue">The new value, or null if the value is being removed.</param>
    public void OnSettingChange(Terminal terminal, ISettings settings, string settingName, object newValue);
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
        await Parallel.ForEachAsync(files, (async (file, stop) =>
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
        }));

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

    public PluginManager()
    {
        Plugins = new();
        Loader = new PluginLoader();
    }

    public Task LoadPlugins(Terminal terminal)
    {
        var plugins = Loader.LoadFromPath(Path.Combine(terminal.Settings.SavePath, "plugins")).Result;
        foreach (var plugin in plugins)
        {
            // assign each a unique Guid.
            plugin.Id = Guid.NewGuid();
            Plugins.Add(plugin.Id, new PluginData { Plugin = plugin });
            plugin.OnLoaded(terminal);

            Logger().LogWarning(this, $"The `{plugin.Name}` plugin has been loaded with the id `{plugin.Id}`");
        }
        return Task.CompletedTask;
    }

    public Task LoadSinglePlugin(Terminal terminal, string path)
    {
        var plugin = Loader.LoadSinglePlugin(path);
        if (plugin is null)
        {
            terminal.Ui.DisplayLine($"failed to load plugin from path `{path}`");
            return Task.CompletedTask;
        }
        plugin.Id = Guid.NewGuid();
        Plugins.Add(plugin.Id, new PluginData { Plugin = plugin });
        plugin.OnLoaded(terminal);

        Logger().LogWarning(this, $"The `{plugin.Name}` plugin has been loaded with the id `{plugin.Id}`");

        return Task.CompletedTask;
    }

    public Task UnloadPlugins(Terminal terminal)
    {
        foreach (var plugin in Plugins)
        {
            plugin.Value.Plugin.OnUnloaded(terminal);
        }

        Plugins.Clear();
        return Task.CompletedTask;
    }

    public Task UnloadSinglePlugin(Terminal terminal, Guid id)
    {
        if (!Plugins.ContainsKey(id))
            return Task.CompletedTask;

        var data = Plugins[id];
        data.Plugin.OnUnloaded(terminal);
        Plugins.Remove(id);
        return Task.CompletedTask;
    }

    public Task<bool> ActivatePlugin(Terminal terminal, Guid id)
    {
        if (!Plugins.ContainsKey(id))
            return Task.FromResult(false);

        var data = Plugins[id];
        data.Active = true;
        return Task.FromResult(true);
    }

    public Task<bool> DeactivatePlugin(Terminal terminal, Guid id)
    {
        if (!Plugins.ContainsKey(id))
            return Task.FromResult(false);

        var data = Plugins[id];
        data.Active = false;
        return Task.FromResult(true);
    }

    public Task<bool> OnUserInput(Terminal terminal, string input)
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

    public void OnCommandExecuted(Terminal terminal, ICommand command)
    {
        foreach (var (_, data) in Plugins)
        {
            if (!data.Active)
                continue;

            data.Plugin.OnCommandExecuted(terminal, command);
        }
    }

    public void OnSettingChange(Terminal terminal, ISettings settings, string settingName, object newValue)
    {
        foreach (var (_, data) in Plugins)
        {
            if (!data.Active)
                continue;

            data.Plugin.OnSettingChange(terminal, settings, settingName, newValue);
        }
    }
}