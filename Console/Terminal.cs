using Console.Commands;
using Console.Extensions;
using Console.UserInterface;
using Console.UserInterface.UiTypes;
using Console.Utilitys.Options;
using Pastel;
using System.Drawing;
using System.Text;

namespace Console;

public class Terminal
{
    public string WorkingDirectory { get; set; } = Environment.CurrentDirectory;

    public string UnixStyleWorkingDirectory
        => string.Join("", WorkingDirectory.Skip(2)).Replace("\\", "/");
    
    public static string UserMachineName => Environment.MachineName;
    public static string User => Environment.UserName;
    public string WdUmDisplay => BuildPromptPointer();
    
    public IUserInterface Ui { get; }
    public ICommandCentre Commands { get; }
    public ISettings Settings { get; }

    public Terminal(UiType type)
    {
        Ui = UserInterface.Ui.Create(type, this);
        Commands = new BaseCommandCentre();

        if (!Directory.Exists("saved"))
            Directory.CreateDirectory("saved");

        Settings = new ConsoleOptions("saved/options.json");
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
    
    // wrappers for IUserInterface

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

            switch (input.Length)
            {
                case 0:
                    WriteLine("\n");
                    continue;
                case 1:
                    lastResult =
                        Commands.Run(input[0], 
                            Array.Empty<string>().ToList(), 
                            this);
                    break;
                case > 1:
                {
                    var args = input[1..].ToList();
                    lastResult = Commands.Run(
                        input[0],
                        args,
                        this);
                    break;
                }
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
        string? shouldShowOption
            = Settings.GetOptionValue<string>(ConsoleOptions.Setting_ShowBlock);

        if (!bool.TryParse(shouldShowOption, out bool shouldShow))
        {
            shouldShow = true;
        }

        if (shouldShow)
        {
            var blockColor
                = Settings.GetOptionValue<Color>(ConsoleOptions.Setting_BlockColor);

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
}