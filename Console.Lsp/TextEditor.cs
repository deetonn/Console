
using Console.UserInterface;
using Spectre.Console;
using System.Text;

namespace Console.Editor;

public class CursorPosition
{
    public CursorPosition(int line, int column)
    {
        Line = line;
        Column = column;
    }

    public int Line { get; set; }
    public int Column { get; set; }
}

public class LineContent
{
    /// <summary>
    /// The actual data being displayed on the UI.
    /// </summary>
    public StringBuilder Data { get; set; } = new();
}

public class TextEditor
{
    public CursorPosition Position { get; }

    public List<LineContent> Contents { get; }
    public Mutex ContentMutex { get; }
    public IUserInterface Ui { get; }
    public Thread? UserInput { get; internal set; }

    public FileInfo SaveLocation { get; }

    public bool Continue { get; set; }

    public TextEditor(IUserInterface ui, FileInfo file)
    {
        Ui = ui;
        Contents = new List<LineContent>();

        if (!file.Exists)
        {
            throw new FileNotFoundException($"cannot open file '{file.Name}', it does not exist.");
        }

        var lines = File.ReadAllLines(file.FullName);
        Array.ForEach(lines, x =>
        {
            Contents.Add(new LineContent()
            {
                Data = new StringBuilder(x)
            });
        });

        Position = new CursorPosition(0, 0);
        ContentMutex = new Mutex();
        SaveLocation = new FileInfo(file.FullName);
    }

    public string OmitColorTags(string data)
    {
        var builder = new StringBuilder();
        bool isInTag = false;

        foreach (char c in data)
        {
            if (isInTag)
            {
                if (c == ']')
                {
                    isInTag = false;
                    continue;
                }

                continue;
            }

            if (c == '[')
            {
                isInTag = true;
                continue;
            }

            builder.Append(c);
        }

        return builder.ToString();
    }

    /// <summary>
    /// This function requires the aqquisition of the mutex.
    /// </summary>
    /// <returns></returns>
    public string[] ExtractRealFileLines()
    {
        var lines = new string[Contents.Count];

        // We need to omit all colour tags.
        for (var i = 0; i < Contents.Count; i++)
        {
            var ommited = OmitColorTags(Contents[i].Data.ToString());
            lines[i] = ommited;
        }

        return lines;
    }

    public void SaveFile()
    {
        ContentMutex.WaitOne();

        if (!SaveLocation.Exists)
        {
            try
            {
                File.Create(SaveLocation.FullName).Dispose();
            }
            catch (Exception)
            {
                throw new NotImplementedException("add error message for when this happens.");
            }
        }

        try
        {
            var lines = ExtractRealFileLines();
            File.WriteAllLines(SaveLocation.FullName, lines.ToArray());
        }
        catch (Exception)
        {
            throw new NotImplementedException("add an error message for when this happens");
        }

        ContentMutex.ReleaseMutex();
    }

    public void LaunchUserInputThread()
    {
        UserInput = new Thread(() =>
        {
            ConsoleKeyInfo keyInfo;
            // F2 signifies HARD quit.
            while ((keyInfo = Ui.GetKey()).Key != ConsoleKey.F2)
            {
                if (keyInfo.Key == ConsoleKey.O
                    && keyInfo.Modifiers == ConsoleModifiers.Control)
                {
                    SaveFile();
                    continue;
                }

                // The key is a normal insertion, aqquire mutex,
                // then insert the character into Contents[Location.Line].Data
                // and increment Location.Column.

                var aqquireMutex = ContentMutex.WaitOne();

                Contents[Position.Line - 1].Data[Position.Column] = keyInfo.KeyChar;
                Position.Column += 1;

                ContentMutex.ReleaseMutex();
            }
        });
    }

    public void Render()
    {
        // Launch user input thread.

        while (Continue)
        {
            var maxLineNoSpacing = Contents.Count.ToString().Length;

            // We just need to re-draw the contents.
            // For this we use markdown, within AnsiConsole.
            // NO EDITING IS DONE HERE. So mutex is not needed.
            for (int i = 0, line = 1; i < Contents.Count; ++i)
            {
                var lineContents = Contents[i];
                var spacingAfterLineNo = new string(' ', maxLineNoSpacing - line.ToString().Length);

                AnsiConsole.MarkupLine($"[bold]{line}[/]{spacingAfterLineNo}|{lineContents}");
            }
        }
    }
}
