using System.Drawing;
using Pastel;
using Console.UserInterface.UiTypes;

namespace Console.UserInterface;

public class NativeConsoleUi : IUserInterface
{
    public NativeConsoleUi()
    {
        Tray = new ConsoleMessageTray();
        System.Console.Clear();
    }
    
    public void DisplayLine(string message, Severity type = Severity.None)
    {
        var toStr = type.ToString();
        
        var msg = type switch
        {
            Severity.None =>$"[{"cmd".Pastel(Color.Gray)}] {message}",
            Severity.Information => $"({toStr.Pastel(Color.Aqua)}) {message}",
            Severity.Error => $"({toStr.Pastel(Color.Red)}) {message}",
            Severity.Critical => $"({toStr.Pastel(Color.OrangeRed)}) {message}",
            _ => $"({toStr.Pastel(Color.Aqua)}) {message}"
        };
        
        System.Console.WriteLine(msg);
    }

    public void Display(string message, Severity type = Severity.None)
    {
        var msg = type switch
        {
            Severity.None =>message,
            _ => $"({type}) {message}"
        };
        
        System.Console.Write(msg);
    }

    public void DisplayLinePure(string message)
    {
        System.Console.WriteLine(message);
    }

    public void DisplayPure(string message)
    {
        System.Console.Write(message);
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

    public IMessageTray Tray { get; }
}