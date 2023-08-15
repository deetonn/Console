using Console.Commands;
using Console.Commands.Builtins.Web.WebServer;
using Console.Events;
using Console.Extensions;
using Console.Plugins;
using Console.UserInterface;
using Console.UserInterface.Input;
using Console.UserInterface.UiTypes;
using Console.Utilitys.Configuration;
using Console.Utilitys.Options;
using Spectre.Console;
using System.Drawing;
using System.Text;

using SystemColor = System.Drawing.Color;
using SystemConsole = global::System.Console;

namespace Console;

public interface IConsole
{
    public string WorkingDirectory { get; set; }
    public string UnixStyleWorkingDirectory { get; }
    public string WdUmDisplay { get; }
    public IUserInterface Ui { get; }
    public ICommandCentre Commands { get; }
    public ISettings Settings { get; set; }
    public IPluginManager PluginManager { get; }
    public IServer? Server { get; set; }
    public IInputHandler InputHandler { get; }
    public IConfiguration Config { get; }
    public IEventHandler EventHandler { get; }

    public string GetConfigPath();
}

public class Terminal : IDisposable, IConsole
{
    public const string GithubLink = "https://github.com/deetonn/Console";

    public string WorkingDirectory { get; set; }
    public string UnixStyleWorkingDirectory => WorkingDirectory[2..].Replace("\\", "/");

    public static string UserMachineName => Environment.MachineName;
    public static string User => Environment.UserName;
    public string WdUmDisplay => BuildPromptPointer();
    /// <summary>
    /// This points to the configuration folder.
    /// </summary>
    public readonly string ConfigurationPath;

    public string GetConfigPath() => ConfigurationPath;

    public IUserInterface Ui { get; }
    public ICommandCentre Commands { get; }
    public ISettings Settings { get; set; }
    public IPluginManager PluginManager { get; internal set; }
    public IServer? Server { get; set; }
    public IInputHandler InputHandler { get; internal set; }
    public IConfiguration Config { get; internal set; }

    public IEventHandler EventHandler { get; }

    public readonly string SavePath;

    public Terminal(UiType type)
    {
        EventHandler = new GlobalEventHandler();

        ConfigurationPath = SortConfigPath();
        SavePath = Path.Combine(ConfigurationPath, "options.json");

        Config = new Configuration();

        var prevDirectory = Environment.CurrentDirectory;
        Environment.CurrentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        WorkingDirectory = Environment.CurrentDirectory;

        Logger().LogInfo(this, $"Initialized the working directory to `{WorkingDirectory}`");

        Ui = UserInterface.Ui.Create(type, this);
        Commands = new BaseCommandCentre();
        InputHandler = new InputHandler();

        // Call the OnInit function for each BaseBuiltinCommand.
        Commands.Elements.Select(x => x as BaseBuiltinCommand)
            .ToList()
            .ForEach(x => x?.OnInit(this));

        EnsurePluginsDirectory();

        Settings = new ConsoleOptions(SavePath, this);

        Environment.CurrentDirectory = prevDirectory;
        WorkingDirectory = Environment.CurrentDirectory;

        PluginManager = new PluginManager(this);
        PluginManager.LoadPlugins(this);

        Logger().LogInfo(this, $"Main terminal instance ready. [{this}]");

        EventHandler.HandleOnApplicationStart(new(this));
    }

    private void EnsurePluginsDirectory()
    {
        var pluginsDirectory = Path.Combine(ConfigurationPath, "plugins");
        if (!Directory.Exists(pluginsDirectory))
        {
            Directory.CreateDirectory(pluginsDirectory);
        }
    }

    public string BuildPromptPointer()
    {
        var sb = new StringBuilder();

        var userNameColor = Settings.GetOptionValue<SystemColor>(ConsoleOptions.Setting_UserNameColor);
        var machineNameColor = Settings.GetOptionValue<SystemColor>(ConsoleOptions.Setting_MachineNameColor);

        sb.Append($"[italic][{userNameColor.ToHexString()}]{User}[/][/]");
        sb.Append('@');
        sb.Append($"[{machineNameColor.ToHexString()}]{UserMachineName}[/]");
        sb.Append($"~{UnixStyleWorkingDirectory}$");

        return sb.ToString();
    }

    // Wrappers for IUserInterface
    public IMessageTray GetTray() => Ui.Tray;

    public void WriteLine(string message = "")
        => Ui.DisplayLineMarkup(message);

    internal void MainLoop()
    {
#if DEBUG
        SystemConsole.WriteLine("[DEBUG]: Waiting for input and displaying errors..");
        SystemConsole.ReadKey();
#endif

        SystemConsole.Clear();
        var lastResult = 0;

        WriteLine($"Welcome to [cyan]Console[/]. Type [red]help[/] to get started with commands.");
        WriteLine($"This application is open source, and can be found at [link={GithubLink}]it's repository[/].\n\n");

        while (lastResult != CommandReturnValues.SafeExit)
        {
            var prompt = WdUmDisplay + " ";
            var initialInput = Ui.GetLine(prompt).Split();
            var input = HandleInlineEnvironmentVariables(initialInput);

            if (input is null)
            {
                Logger().LogError(this, "after inlining environment variables the result came back null?");
                WriteLine($"Sorry, an error occured. LastInput: {string.Join(" ", initialInput)}");
                continue;
            }

            if (!HandleOnInputEvent(input))
                continue;

            if (input.Length >= 1 && input[0].StartsWith("./"))
            {
                lastResult = DoDotSlashCommand(input.ToList());
                // Fuck my life
                goto AfterProcessing;
            }

            switch (input.Length)
            {
                case 0:
                    WriteLine("\n");
                    continue;
                case 1:
                    lastResult = Commands.Run(input[0], Array.Empty<string>().ToList(), this);
                    break;
                default:
                    var args = ProcessInputArgs(input[1..]);
                    lastResult = Commands.Run(input[0], args.ToList(), this);
                    break;
            }

        AfterProcessing:
            if (string.IsNullOrEmpty(input[0]))
                lastResult = CommandReturnValues.DontShowText;

            var translation = Result.Translate(lastResult);
            if (!string.IsNullOrEmpty(translation))
                Ui.DisplayLine($"{translation}");
        }

        WriteLine("safe quit has began, [italic]exiting[/]");
    }

    public string[] GetInput(string prompt)
    {
        DisplayBlock();
        Ui.Display($"{WdUmDisplay} ");
        return Ui.GetLine(prompt).Split(' ');
    }

    public void DisplayBlock()
    {
        string? shouldShowOption = Settings.GetOptionValue<string>(ConsoleOptions.Setting_ShowBlock);

        if (!bool.TryParse(shouldShowOption, out bool shouldShow))
        {
            shouldShow = true;
        }

        if (shouldShow)
        {
            var blockColor = Settings.GetOptionValue<SystemColor>(ConsoleOptions.Setting_BlockColor);

            const char space = ' ';
            System.Console.BackgroundColor = blockColor.ClosestConsoleColor();
            Ui.DisplayLinePure(new string(space, System.Console.BufferWidth));
            System.Console.ResetColor();
        }
    }

    public static SystemColor MakeColorFromHexString(string hexString)
    {
        return ColorTranslator.FromHtml(hexString);
    }

    public override string ToString()
    {
        return $"Terminal(User={User})";
    }

    private string SortConfigPath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var configPath = Path.Combine(appData, "Console");
        Directory.CreateDirectory(configPath);

        var realPath = Path.Combine(configPath, "saved");
        Directory.CreateDirectory(realPath);

        return realPath;
    }

    private string[]? HandleInlineEnvironmentVariables(string[]? input)
    {
        // If the input is null, return it directly
        if (input is null)
            return input;

        // Join the array into a single string with space as separator
        var full = string.Join(' ', input);

        // Initialize a StringBuilder to store the result
        var result = new StringBuilder();

        // Loop through the entire input string
        for (int i = 0; i < full.Length; ++i)
        {
            char current = full[i];

            // Check for the start of an environment variable (marked by '{')
            if (current == '{')
            {
                // Initialize a StringBuilder to store the environment variable name
                var currentVar = new StringBuilder();

                // Move to the next character and start reading the variable name
                while (true)
                {
                    // Skip the first '{'
                    if (current == '{')
                    {
                        current = full[++i];
                        continue;
                    }

                    // Stop reading when we encounter the closing '}'
                    if (current == '}')
                    {
                        // Skip the closing '}' and move to the next character
                        if (i + 1 >= full.Length)
                        {
                            break;
                        }
                        current = full[++i];
                        break;
                    }

                    // Append the current character to the variable name
                    currentVar.Append(current);
                    current = full[++i];
                }

                // Convert the StringBuilder to a string (the variable name)
                var varName = currentVar.ToString();
                var value = Environment.GetEnvironmentVariable(varName);

                // Check if the environment variable exists
                if (value is null)
                {
                    // Get the "execution.strictmode" setting
                    bool? isStrictCall = Settings.GetOptionValue<bool>("execution.strictmode");

                    // Check if strict mode is enabled
                    if (isStrictCall.HasValue && isStrictCall.Value)
                    {
                        // Print an error message and return the original input
                        WriteLine($"ERROR: environment variable `{varName}` does not exist. Therefore it cannot be inserted inline.");
                        return input;
                    }
                    else
                    {
                        // If not in strict mode, append the variable name as-is
                        result.Append(varName);
                    }
                }
                else
                {
                    // If the environment variable exists, append its value
                    result.Append(value);
                }

                // Continue to the next character in the input
                continue;
            }

            // Append the character to the result
            result.Append(current);
        }

        // Split the final result string into an array of strings and return it
        var res = result.ToString().Split();
        return res;
    }

    private bool HandleOnInputEvent(string[]? data)
    {
        if (data == null)
        {
            return true; // No input, add a newline.
        }

        var asString = string.Join(' ', data);
        return EventHandler.HandleOnUserInput(new(asString));
    }

    private int DoDotSlashCommand(List<string> data)
    {
        var fileName = data[0].Replace("./", "");
        var fullPath = Path.Combine(WorkingDirectory, fileName);

        if (!File.Exists(fullPath))
        {
            Ui.DisplayLine($"The file `{fileName}` does not exist in the current directory.");
            return -1;
        }

        var arguments = new List<string>() { fullPath };
        data.Skip(1).ToList().ForEach(arguments.Add);
        return Commands.ExecuteFrom(this, "run", arguments.ToArray());
    }

    private List<string> ProcessInputArgs(string[] args)
    {
        var result = new List<string>();

        for (uint i = 0; i < args.Length; ++i)
        {
            var arg = args[i];

            if (arg.StartsWith("\""))
            {
                var argBuilder = new StringBuilder();
                argBuilder.Append(arg[1..]);

                while (true)
                {
                    if (arg.EndsWith("\""))
                    {
                        argBuilder.Remove(argBuilder.Length - 1, 1);
                        break;
                    }

                    argBuilder.Append(' ');
                    argBuilder.Append(args[++i]);
                    arg = args[i];
                }

                result.Add(argBuilder.ToString());
            }
            else
            {
                result.Add(args[i]);
            }
        }

        return result;
    }

    public void Dispose()
    {
        // unload all plugins.
        GC.SuppressFinalize(this);

        WriteLine("");

        AnsiConsole.Status()
            .Spinner(Spinner.Known.Star)
            .Start("Closing...", ctx => {
                // Omitted
                EventHandler.HandleOnApplicationExit(new(this));
                PluginManager.UnloadPlugins(this);
            });
    }
}
