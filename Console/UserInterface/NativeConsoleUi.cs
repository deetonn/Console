using Console.UserInterface.UiTypes;
using Console.Utilitys.Options;
using Pastel;
using Spectre.Console;
using NativeColor = System.Drawing.Color;
using SystemConsole = System.Console;

namespace Console.UserInterface;

public class NativeConsoleUi : IUserInterface
{
    private readonly Terminal parent;

    public NativeConsoleUi(Terminal parent)
    {
        Tray = new ConsoleMessageTray();
        Clear();

        this.parent = parent;

        AnsiConsole.Decoration = Decoration.Italic;

        Logger().LogInfo(this, "Native console interface is being used.");
        Logger().LogInfo(this, $"Parent terminal service is `{parent}`");
    }

    public bool ShouldDisplayWatermark()
    {
        var setting = parent.Settings.GetOptionValue<string>(
            ConsoleOptions.Setting_DisplayWatermark);

        if (!bool.TryParse(setting, out bool result))
            return false;
        return result;
    }

    public void DisplayLine(string message, Severity type = Severity.None)
    {
        var toStr = type.ToString();

        var watermarkColor =
            parent.Settings.GetOptionValue<NativeColor>(ConsoleOptions.Setting_WatermarkColor);
        var textColor =
            parent.Settings.GetOptionValue<NativeColor>(ConsoleOptions.Setting_TextColor);

        if (ShouldDisplayWatermark())
        {
            var msg = type switch
            {
                Severity.None => $"[{"cmd".Pastel(watermarkColor)}] {message.Pastel(textColor)}",
                Severity.Information => $"({toStr.Pastel(NativeColor.Aqua)}) {message.Pastel(textColor)}",
                Severity.Error => $"({toStr.Pastel(NativeColor.Red)}) {message.Pastel(textColor)}",
                Severity.Critical => $"({toStr.Pastel(NativeColor.OrangeRed)}) {message.Pastel(textColor)}",
                _ => $"({toStr.Pastel(NativeColor.Aqua)}) {message.Pastel(textColor)}"
            };

            SystemConsole.WriteLine(msg);
        }
        else
        {
            var msg = type switch
            {
                Severity.None => $"{message.Pastel(textColor)}",
                Severity.Information => $"({toStr.Pastel(NativeColor.Aqua)}) {message.Pastel(textColor)}",
                Severity.Error => $"({toStr.Pastel(NativeColor.Red)}) {message.Pastel(textColor)}",
                Severity.Critical => $"({toStr.Pastel(NativeColor.OrangeRed)}) {message.Pastel(textColor)}",
                _ => $"({toStr.Pastel(NativeColor.Aqua)}) {message.Pastel(textColor)}"
            };

            SystemConsole.WriteLine(msg);
        }
    }

    public void Display(string message, Severity type = Severity.None)
    {
        var textColor =
            parent.Settings.GetOptionValue<NativeColor>(ConsoleOptions.Setting_TextColor);

        var msg = type switch
        {
            Severity.None => message.Pastel(textColor),
            _ => $"({type}) {message.Pastel(textColor)}"
        };

        SystemConsole.Write(msg);
    }

    public void DisplayLinePure(string message)
    {
        var textColor =
            parent.Settings.GetOptionValue<NativeColor>(ConsoleOptions.Setting_TextColor);
        SystemConsole.WriteLine(message.Pastel(textColor));
    }

    public void DisplayPure(string message)
    {
        var textColor =
            parent.Settings.GetOptionValue<NativeColor>(ConsoleOptions.Setting_TextColor);
        SystemConsole.Write(message.Pastel(textColor));
    }

    public void SetTitle(string message)
    {
        try
        {
            var count = Tray.Messages.Count;
            var pending = (count < 0);

            SystemConsole.Title = pending
                ? message
                : $"{count} notifications | {message}";
        }
        catch (PlatformNotSupportedException)
        {
            var platform = Environment.OSVersion.Platform;
            DisplayLine($"`{platform}` is not supported for SetTitle", Severity.Error);
        }
    }

    public void Clear()
    {
        AnsiConsole.Clear();
    }

    public string GetLine(string prompt)
    {
        DisplayMarkup(prompt);
        var input = SystemConsole.ReadLine();
        DisplayPure("\n");
        return input ?? "Ctrl+Z";
    }

    public ConsoleKeyInfo GetKey()
    {
        // Must intercept.
        return SystemConsole.ReadKey(true);
    }

    public void DisplayMarkup(string markup)
    {
        AnsiConsole.Markup($"[white]{markup}[/]");
    }

    public void DisplayLineMarkup(string markup)
    {
        AnsiConsole.MarkupLine($"[white]{markup}[/]");
    }

    public IMessageTray Tray { get; }
}