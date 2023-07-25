
// Necessary to avoid repetitive code fetching singleton instances.
global using static Console.Logging.StaticLoggerGlobalUseMe;

using Console;
using Console.Commands.Scripting;
using Console.UserInterface.UiTypes;
using Console.Utilitys;

Main();

void Main()
{
    Singleton<ILogger>.InitTo(instance: new ConsoleLogger());

    var wantsUi = args.Contains(value: "--gui");

    var terminal = new Terminal(
        // These are the only two UiTypes.
        type: wantsUi ? UiType.ImGui : UiType.Console
    );

    var doesWantScript = args.Contains(value: "--script");
    if (doesWantScript)
    {
        var scriptFile = Array.Find(args, arg => arg.StartsWith(value: "--script="))?.Split(separator: "=")[1];
        if (scriptFile == null)
        {
            Logger().LogError("Main", "No script file was specified.");
            return;
        }

        var context = new ScriptExecutionContext(terminal, scriptFile, (message) =>
        {
            terminal.WriteLine($"Failure: {message}");
            Environment.Exit(0);
        });

        context.Execute(terminal);
    }

    terminal.MainLoop();
}