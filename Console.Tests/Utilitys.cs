
using Console.Commands;
using Console.Commands.Builtins.Web.WebServer;
using Console.Events;
using Console.Plugins;
using Console.UserInterface;
using Console.UserInterface.Input;
using Console.UserInterface.UiTypes;
using Console.Utilitys;
using Console.Utilitys.Configuration;
using Console.Utilitys.Options;

using SystemConsole = System.Console;

namespace Console.Tests;

public class DummyUserInterface : IUserInterface
{
    public IMessageTray Tray => throw new NotImplementedException();

    public void Clear()
    {
        SystemConsole.Clear();
    }

    public void Display(string message, Severity type = Severity.None)
    {
        SystemConsole.Write(message);
    }

    public void DisplayLine(string message, Severity type = Severity.None)
    {
        SystemConsole.WriteLine(message);
    }

    public void DisplayLineMarkup(string markup)
    {
        // This test suite does not care about the markup, just display it.
        DisplayLine(markup);
    }

    public void DisplayLinePure(string message)
    {
        DisplayLine(message);
    }

    public void DisplayMarkup(string markup)
    {
        Display(markup);
    }

    public void DisplayPure(string message)
    {
        Display(message);
    }

    public ConsoleKeyInfo GetKey()
    {
        throw new NotImplementedException();
    }

    public string GetLine(string prompt)
    {
        throw new NotImplementedException();
    }

    public void SetTitle(string message)
    {
        SystemConsole.Title = message;
    }
}

public class NonfunctionalTerminal : IConsole
{
    public string WorkingDirectory { get; set; } = Directory.GetCurrentDirectory();

    public string UnixStyleWorkingDirectory => throw new NotImplementedException();

    public string WdUmDisplay => throw new NotImplementedException();

    public IUserInterface Ui { get; } = new DummyUserInterface();

    public ICommandCentre Commands { get; } = new BaseCommandCentre();

    public ISettings Settings { get; set; }

    public IPluginManager PluginManager { get; }

    // Ignore, not a main part of this project.
    public IServer? Server { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    // Unused
    public IInputHandler InputHandler => throw new NotImplementedException();

    public IConfiguration Config { get; } = new Configuration();

    public IEventHandler EventHandler { get; } = new GlobalEventHandler();

    public string GetConfigPath()
    {
        // For tests, the config path should be contained.
        // Using the real, one could pollute developers configuration.
        return Directory.GetCurrentDirectory();
    }

    public NonfunctionalTerminal()
    {
        Settings = new ConsoleOptions(Directory.GetCurrentDirectory(), this)
        {
            IsTestMode = true
        };
        PluginManager = new PluginManager(this);
    }
}

public static class DummyConsole
{
    public static IConsole Get => new NonfunctionalTerminal();

    static DummyConsole()
    {
        Singleton<ILogger>.InitTo(new ConsoleLogger());
    }
}
