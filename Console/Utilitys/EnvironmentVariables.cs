using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Console.Utilitys;

public class EnvironmentVariables : IEnvironmentVariables
{
    public Dictionary<string, string> Variables { get; }

    public EnvironmentVariables()
    {
        Variables = new Dictionary<string, string>();
        Clear();
    }

    public string? Get(string identifier)
    {
        if (!Variables.TryGetValue(identifier.ToLower(), out var value))
            return null;
        return value;
    }

    public bool TryGet(string identifier, [NotNullWhen(true)] out string? value)
    {
        value = Get(identifier);
        return identifier != null;
    }

    public void Set(string identifier, string key)
    {
        Variables[identifier] = key;
    }

    public void Clear()
    {
        Variables.Clear();
        var variables = (Hashtable)Environment.GetEnvironmentVariables();
        foreach (DictionaryEntry pair in variables)
        {
            // TODO: make this shit look better, wtf is this api design
            if (pair.Key is string key)
            {
                if (pair.Value is string value)
                {
                    Variables.Add(key.ToLower(), value);
                }
            }
        }
    }

    // NOTE: "_" is the return value.
    //       "$" is the output value.

    public void EnterCommandContext()
    {
        Set("$", string.Empty);
        Set("_", "0");
    }

    public void AppendCommandOutput(string output)
    {
        if (Contains("$"))
        {
            Variables["$"] += output;
        }
        // TODO: handle the case where command context has not been
        // entered.
    }

    public void RegisterCommandResult(int result)
    {
        Set("_", result.ToString());
    }

    public bool Contains(string ident) => Variables.ContainsKey(ident);
}
