using System.Drawing;
using Pastel;
using Console.UserInterface.UiTypes;
using Console.Utilitys.Options;

namespace Console.UserInterface;

public class NativeConsoleUi : IUserInterface
{
    private readonly Terminal parent;

    public NativeConsoleUi(Terminal parent)
    {
        Tray = new ConsoleMessageTray();
        System.Console.Clear();

        this.parent = parent;

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
            parent.Settings.GetOptionValue<Color>(ConsoleOptions.Setting_WatermarkColor);
        var textColor =
            parent.Settings.GetOptionValue<Color>(ConsoleOptions.Setting_TextColor);
        
        if (ShouldDisplayWatermark())
        {
            var msg = type switch
            {
                Severity.None => $"[{"cmd".Pastel(watermarkColor)}] {message.Pastel(textColor)}",
                Severity.Information => $"({toStr.Pastel(Color.Aqua)}) {message.Pastel(textColor)}",
                Severity.Error => $"({toStr.Pastel(Color.Red)}) {message.Pastel(textColor)}",
                Severity.Critical => $"({toStr.Pastel(Color.OrangeRed)}) {message.Pastel(textColor)}",
                _ => $"({toStr.Pastel(Color.Aqua)}) {message.Pastel(textColor)}"
            };

            System.Console.WriteLine(msg);
        }
        else
        {
            var msg = type switch
            {
                Severity.None => $"{message.Pastel(textColor)}",
                Severity.Information => $"({toStr.Pastel(Color.Aqua)}) {message.Pastel(textColor)}",
                Severity.Error => $"({toStr.Pastel(Color.Red)}) {message.Pastel(textColor)}",
                Severity.Critical => $"({toStr.Pastel(Color.OrangeRed)}) {message.Pastel(textColor)}",
                _ => $"({toStr.Pastel(Color.Aqua)}) {message.Pastel(textColor)}"
            };

            System.Console.WriteLine(msg);
        }
    }

    public void Display(string message, Severity type = Severity.None)
    {
        var textColor =
            parent.Settings.GetOptionValue<Color>(ConsoleOptions.Setting_TextColor);

        var msg = type switch
        {
            Severity.None => message.Pastel(textColor),
            _ => $"({type}) {message.Pastel(textColor)}"
        };
        
        System.Console.Write(msg);
    }

    public void DisplayLinePure(string message)
    {
        var textColor =
            parent.Settings.GetOptionValue<Color>(ConsoleOptions.Setting_TextColor);
        System.Console.WriteLine(message.Pastel(textColor));
    }

    public void DisplayPure(string message)
    {
        var textColor =
            parent.Settings.GetOptionValue<Color>(ConsoleOptions.Setting_TextColor);
        System.Console.Write(message.Pastel(textColor));
    }

    public void SetTitle(string message)
    {
        try
        {
            var count = Tray.Messages.Count;
            var pending = (count < 0);

            System.Console.Title = pending
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
        System.Console.Clear();
    }

    public string GetLine()
    {
        var input = System.Console.ReadLine();
        return input ?? "Ctrl+Z";
    }

    public ConsoleKeyInfo GetKey()
    {
        return System.Console.ReadKey(true);
    }

    public void Erase(int count)
    {
        // go back from the current console position and clear count amount
        // of characters
        for (var i = 0; i < count; i++)
        {
            System.Console.Write("\b \b");
        }
    }

    public IMessageTray Tray { get; }
}