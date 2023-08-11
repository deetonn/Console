using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SysPath = global::System.IO.Path;
using NoDiscard = System.Diagnostics.Contracts.PureAttribute;

namespace Console.Utilitys.Configuration;

public class ConfigFile
{
    public string Path { get; }
    public bool WasJustCreated { get; internal set; }

    public ConfigFile(string path, bool wasJustCreated)
    {
        Path = path;
        WasJustCreated = wasJustCreated;
    }

    /// <summary>
    /// Overwrite the file contents with <paramref name="data"/>.
    /// </summary>
    /// <param name="data">The data to write to the file</param>
    public void WriteAll(string data)
    {
        File.WriteAllText(Path, data);
    }

    /// <summary>
    /// Append text to file, leaving the existing contents.
    /// </summary>
    /// <param name="data"></param>
    public void AppendAll(string data)
    {
        File.AppendAllText(Path, data);
    }

    /// <summary>
    /// Read the contents from the file.
    /// </summary>
    /// <returns>The file contents.</returns>
    public string Read()
    {
        return File.ReadAllText(Path);
    }

    /// <summary>
    /// Attempt to <see cref="Read"/>, then deserialize the data
    /// into an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to</typeparam>
    /// <returns>The deserialized data if succesful, otherwise null.</returns>
    public T? Deserialize<T>()
    {
        return JsonConvert.DeserializeObject<T>(Read());
    }
}

/// <summary>
/// This is basically a wrapper for a folder in the config folder.
/// </summary>
public class ConfigSection
{
    public string Path { get; }
    public bool WasJustCreated { get; internal set; } = false;

    public ConfigSection(string path)
    {
        Path = path;
    }

    /// <summary>
    /// Create a file in your modules specific config folder.
    /// </summary>
    /// <param name="name">The name of the file, excluding extension.</param>
    /// <returns></returns>
    public ConfigFile MakeFile(string name)
    {
        // all files will be saved under the .config extension.
        var fullPath = SysPath.Combine(Path, $"{name}.config");

        if (!File.Exists(fullPath))
        {
            WasJustCreated = true;
            File.Create(fullPath).Dispose();
        }

        return new ConfigFile(fullPath, WasJustCreated);
    }
    public ConfigFile GetFile(string name)
    {
        var fullPath = SysPath.Combine(Path, $"{name}.config");

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"Could not find file {name}.config in {Path}. Did you make sure to create it?");

        return new ConfigFile(fullPath, WasJustCreated);
    }
}

public interface IConfiguration
{
    /// <summary>
    /// All configuration sections that have been made.
    /// </summary>
    public IDictionary<string, ConfigSection> Sections { get; }

    /// <summary>
    /// Make your own section in the config folder. You should only do this
    /// once per module. Keep hold of the return value. This function will create
    /// the folder if it does not exist, and will return the existing folder if it does.
    /// </summary>
    /// <param name="name">The name of your module.</param>
    /// <returns>The config section.</returns>
    [NoDiscard]
    public ConfigSection MakeSection(IConsole parent, string name);
}

public class Configuration : IConfiguration
{
    public IDictionary<string, ConfigSection> Sections { get; } = new Dictionary<string,  ConfigSection>();

    public ConfigSection MakeSection(IConsole parent, string name)
    {
        var configPath = parent.GetConfigPath();
        // get the full path to plugin config. [ConfigPath]/plugins/config
        var fullPath = SysPath.Combine(configPath, "plugins", "config");

        // Ignore Directory.Exists() due to CreateDirectory already
        // having this behaviour.
        Directory.CreateDirectory(fullPath);

        // Add a new directory for the module.
        var sectionPath = SysPath.Combine(fullPath, name);
        Directory.CreateDirectory(sectionPath);

        var section = new ConfigSection(sectionPath);

        Sections.Add(name, section);

        return section;
    }
}
