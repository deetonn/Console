using Console.Commands;
using Console.Commands.Builtins.Web.WebServer;
using Console.Extensions;
using Console.Plugins;
using Console.UserInterface;
using Console.UserInterface.UiTypes;
using Console.Utilitys.Options;
using Pastel;
using System.Drawing;
using System.Text;

namespace Console
{
    public class Terminal
    {
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

            while (lastResult != CommandReturnValues.SafeExit)
            {
                var input = GetInput();

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
