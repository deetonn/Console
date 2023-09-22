using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console.Utilitys;

/// <summary>
/// This serves as an interface for getting, setting and reloading
/// environment variables.
/// </summary>
public interface IEnvironmentVariables
{
    public Dictionary<string, string> Variables { get; }

    /// <summary>
    /// Attempt to get an environment variable under the name of 
    /// <paramref name="identifier"/>.
    /// </summary>
    /// <param name="identifier">The environment variable to try and find.</param>
    /// <returns>The variable, or null if it doesnt exist.</returns>
    public string? Get(string identifier);

    /// <summary>
    /// Attempt to get an environment variable under the name of 
    /// <paramref name="identifier"/>.
    /// </summary>
    /// <param name="identifier">The environment variable to try and get</param>
    /// <param name="value">The value, if there is one</param>
    /// <returns>true if it was found, otherwise false.</returns>
    public bool TryGet(string identifier, [NotNullWhen(true)] out string? value);

    /// <summary>
    /// Add or set a variable.
    /// </summary>
    /// <param name="identifier">The identifier</param>
    /// <param name="value">The value to set or add</param>
    public void Set(string identifier, string value);

    /// <summary>
    /// Clears the environment variables and reloads the default system ones.
    /// </summary>
    public void Clear();

    /// <summary>
    /// This will prepare the environment variables "_" and "$" for
    /// command output.
    /// </summary>
    public void EnterCommandContext();

    /// <summary>
    /// This will append command output into the "$" environment variable.
    /// </summary>
    /// <param name="output">The output to append.</param>
    public void AppendCommandOutput(string output);

    /// <summary>
    /// This will set the previous command result.
    /// </summary>
    /// <param name="result">The result.</param>
    public void RegisterCommandResult(int result);
}
