using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console.Commands.History;

public interface ICommandHistory
{
    /// <summary>
    /// This will contain all executed lines that have been entered into
    /// the Console user interface. Including all arguments.
    /// </summary>
    public List<string> History { get; }

    /// <summary>
    /// The current index with <see cref="History"/> that the history is in.
    /// This should always be set to <see cref="History"/>.Count - 1 after the user
    /// is done scrolling through, or before.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// This signals if the command history was able to initialize correctly, or
    /// if it has been disabled.
    /// </summary>
    public bool Enabled { get; }

    /// <summary>
    /// The most recently returned command from either <see cref="HandleMoveUp"/>
    /// or <see cref="HandleMoveDown"/>. This makes it easier to clear the line.
    /// </summary>
    public string MostRecentlyViewedCommand { get; }

    /// <summary>
    /// This should be called when the user presses the up arrow key.
    /// The index should be decremented by one, and the line at the index before
    /// should be returned.
    /// </summary>
    /// <returns>The next command, or null if there are no more.</returns>
    public string? HandleMoveDown();

    /// <summary>
    /// This should be called when the user presses the down arrow key.
    /// The index should be incremented by one, and the line at the index before
    /// should be returned.
    /// </summary>
    /// <returns>The next command, or null if there are no more.</returns>
    public string? HandleMoveUp();

    /// <summary>
    /// Add a recent command. This will reset the history index and
    /// the users next key-up will show them the previous command.
    /// </summary>
    /// <param name="command">The command string to register.</param>
    public void AddRecentCommand(Terminal parent, string command);

    /// <summary>
    /// Save the command history to the configuration path.
    /// </summary>
    /// <param name="parent"></param>
    public void Save(Terminal parent);
}

public class CommandHistory : ICommandHistory
{
    public const string HistorySaveFileName = "history.json";

    public List<string> History { get; } = new();

    public int Index { get; private set; }

    public bool Enabled { get; private set; } = true;

    public string MostRecentlyViewedCommand { get; private set; } = string.Empty;   

    public CommandHistory(Terminal parent)
    {
        var savedHistoryPath = Path.Join(parent.ConfigurationPath, HistorySaveFileName);

        if (!File.Exists(savedHistoryPath))
        {
            try
            {
                File.Create(savedHistoryPath).Close();
            }
            catch (Exception e)
            {
                Logger().LogError(this, $"Failed to initialize the command history. [{e.Message}]");
                Enabled = false;
            }
        }
        else
        {
            var historyFileContents = File.ReadAllText(savedHistoryPath);
            try
            {
                var contents = JsonConvert.DeserializeObject<List<string>>(historyFileContents);
                if (contents == null)
                {
                    Logger().LogWarning(this, $"command history file is present but has no entries.");
                }
                else
                {
                    History = contents;
                }
            }
            catch (Exception e)
            {
                // This is still fine, but previous commands cannot be loaded.
                Logger().LogError(this, $"failed to load previous saved command history [{e.Message}]");
            }
        }
    }

    public string? HandleMoveDown()
    {
        if (Index - 1 < 0) return null;
        return MostRecentlyViewedCommand = History[--Index];
    }

    public string? HandleMoveUp()
    {
        if (Index + 1 > History.Count - 1) return null;
        return MostRecentlyViewedCommand = History[++Index];
    }

    public void AddRecentCommand(Terminal parent, string command)
    {
        History.Add(command);
        Index = History.Count - 1;

        Save(parent);
    }

    public void Save(Terminal parent)
    {
        var historySavePath = Path.Join(parent.ConfigurationPath, HistorySaveFileName);
        var serialized = JsonConvert.SerializeObject(History);
        File.WriteAllText(historySavePath, serialized);
    }
}
