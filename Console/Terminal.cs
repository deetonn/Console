using Console.Commands;
using Console.Commands.Builtins.Web.WebServer;
using Console.Extensions;
using Console.Plugins;
using Console.UserInterface;
using Console.UserInterface.Input;
using Console.UserInterface.UiTypes;
using Console.Utilitys.Options;
using Pastel;
using System.Drawing;
using System.Text;

namespace Console
{
    public class Terminal
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

        public IUserInterface Ui { get; }
        public ICommandCentre Commands { get; }
        public ISettings Settings { get; internal set; }
        public IPluginManager PluginManager { get; internal set; }
        public IServer? Server { get; set; }
        public IInputHandler InputHandler { get; internal set; }

        public readonly string SavePath;

        public Terminal(UiType type)
        {
            ConfigurationPath = SortConfigPath();
            SavePath = Path.Combine(ConfigurationPath, "options.json");

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

            PluginManager = new PluginManager();
            PluginManager.LoadPlugins(this);

            Logger().LogInfo(this, $"Main terminal instance ready. [{this}]");
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

            var userNameColor = Settings.GetOptionValue<Color>(ConsoleOptions.Setting_UserNameColor);
            var machineNameColor = Settings.GetOptionValue<Color>(ConsoleOptions.Setting_MachineNameColor);

            sb.Append($"{User.Pastel(userNameColor)}");
            sb.Append('@');
            sb.Append($"{UserMachineName.Pastel(machineNameColor)}");
            sb.Append($"~{UnixStyleWorkingDirectory}$");

            return sb.ToString();
        }

        // Wrappers for IUserInterface
        public IMessageTray GetTray() => Ui.Tray;

        public void WriteLine(string message = "", Severity severity = Severity.None)
            => Ui.DisplayLine(message, severity);

        internal void MainLoop()
        {
#if DEBUG
            System.Console.WriteLine("[DEBUG]: Waiting for input and displaying errors..");
            System.Console.ReadKey();
#endif

            System.Console.Clear();
            var lastResult = 0;

            WriteLine($"Welcome to {"Console".Pastel(Color.Cyan)}. Type {"help".Pastel(Color.Red)} to get started with commands.");
            WriteLine($"This application is open source, and can be found at {GithubLink.Pastel(Color.Blue)}\n\n");

            while (lastResult != CommandReturnValues.SafeExit)
            {
                Ui.DisplayPure(WdUmDisplay + " ");
                var initialInput = Ui.GetLine().Split();
                var input = HandleInlineEnvironmentVariables(initialInput);

                if (input is null)
                {
                    Logger().LogError(this, "after inlining environment variables the result came back null?");
                    WriteLine($"Sorry, an error occured. LastInput: {string.Join(" ", initialInput)}");
                    continue;
                }

                if (!HandleOnInputEvent(input))
                    continue;

                switch (input.Length)
                {
                    case 0:
                        WriteLine("\n");
                        continue;
                    case 1:
                        lastResult = Commands.Run(input[0], Array.Empty<string>().ToList(), this);
                        break;
                    default:
                        var args = input[1..];
                        lastResult = Commands.Run(input[0], args.ToList(), this);
                        break;
                }

                if (string.IsNullOrEmpty(input[0]))
                    lastResult = CommandReturnValues.DontShowText;

                var translation = Result.Translate(lastResult);
                if (!string.IsNullOrEmpty(translation))
                    Ui.DisplayLine($"{translation}");
            }
        }

        public string[] GetInput()
        {
            DisplayBlock();
            Ui.Display($"{WdUmDisplay} ");
            return Ui.GetLine().Split(' ');
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
                var blockColor = Settings.GetOptionValue<Color>(ConsoleOptions.Setting_BlockColor);

                const char space = ' ';
                System.Console.BackgroundColor = blockColor.ClosestConsoleColor();
                Ui.DisplayLinePure(new string(space, System.Console.BufferWidth));
                System.Console.ResetColor();
            }
        }

        public static Color MakeColorFromHexString(string hexString)
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
            return PluginManager.OnUserInput(this, asString).Result;
        }
    }
}
