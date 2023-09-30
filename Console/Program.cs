// Necessary to avoid repetitive code fetching singleton instances.
// global using static Console.Logging.StaticLoggerGlobalUseMe;


global using static Console.Logging.StaticLoggerGlobalUseMe;

using Console;
using Console.Commands.Scripting;
using Console.UserInterface.UiTypes;
using Console.Utilitys;
using System.Diagnostics;

Main();

void Main()
{
    Singleton<ILogger>.InitTo(instance: new ConsoleLogger());

    var wantsUi = args.Contains(value: "--gui");
    var isUpdate = args.Contains("--update");

    if (isUpdate)
    {
        // NOTE: This function performs an exit.
        // It will not return.
        FixFileStructure();
    }

    // NOTE: `using` here because Terminal is IDisposable.
    // The cleanup code is always executed this way without
    // a worry.
    using var terminal = new Terminal(
        // These are the only two UiTypes.
        type: wantsUi ? UiType.ImGui : UiType.Console
    );

    if (args.Contains("--jump"))
    {
        var commandToJumpTo = args.Where(x => x.Contains("--jump=")).First().Split("=")[1];
        var commandArguments = args.Where(x => x.Contains("--jump-args=")).FirstOrDefault()?.Split("=")[1..];

        // Just execute previous context then continue within the other process.
        terminal.Commands.ExecuteFrom(terminal, commandToJumpTo, commandArguments ?? Array.Empty<string>());
        Environment.Exit(0);
    }

    // Skip update checks for now, seeing as there is no server to request from.
    // AutoUpdater.CheckForUpdates(terminal);

    var doesWantScript = args.Contains(value: "--script");
    if (doesWantScript)
    {
        var scriptFile = Array.Find(args, arg => arg.StartsWith(value: "--script="))?.Split(separator: "=")[1];
        if (scriptFile == null)
        {
            Logger().LogError(terminal, "No script file was specified.");
            return;
        }

        var context = new ScriptExecutionContext(terminal, scriptFile, (message) =>
        {
            terminal.WriteLine($"Failure: {message}");
            Environment.Exit(0);
        });

        context.Execute(terminal);
    }

    terminal.Run();
}

void FixFileStructure()
{
    if (!Directory.Exists("update"))
    {
        return;
    }
    var cwd = Directory.GetCurrentDirectory();

    var updateFile = Path.Combine(cwd, "console.exe");

    // We need to replace the file in the lower directory
    // with this file.

    var oldExe = Path.Combine("../", cwd, "console.exe");
    var oldCopy = File.ReadAllText(oldExe);

    File.Delete(oldExe);
    File.WriteAllText(oldExe + ".old", oldCopy);

    File.WriteAllText(cwd + "/console.exe", File.ReadAllText(updateFile));

    Process.Start(Path.Combine("../console.exe"));
    Environment.Exit(0);
}
