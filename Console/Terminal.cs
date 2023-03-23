using Console.Commands;
using Console.UserInterface;
using Console.UserInterface.UiTypes;
using Console.Utilitys;
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
        Ui = UserInterface.Ui.Create(type);
        Commands = new BaseCommandCentre();

        if (!Directory.Exists("saved"))
            Directory.CreateDirectory("saved");

        Settings = new ConsoleOptions("saved/options.json");
    }

    public string BuildPromptPointer()
    {
        var sb = new StringBuilder();

        var userNameColor = MakeColorFromHexString(
            Settings.GetOptionValue<string>("colors.username")!);
        var machineNameColor = MakeColorFromHexString(
            Settings.GetOptionValue<string>("colors.machinename")!);

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
        System.Console.Clear();
        var lastResult = 0;

        while (lastResult != CommandReturnValues.SafeExit)
        {
            var input = GetInput();

            switch (input.Length)
            {
                case 0:
                    Ui.DisplayLine("\n");
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
            
            var translation = ResultTranslator.Translate(lastResult);
            if (!string.IsNullOrEmpty(translation))
                Ui.DisplayLine($"{translation}");
        }
    }

    public string[] GetInput()
    {
        Ui.Display($"{WdUmDisplay} ");
        return Ui.GetLine().Split(' ');
    }

    private void DisplayBlock()
    {
        const char space = ' ';
        System.Console.BackgroundColor = ConsoleColor.Magenta;
        Ui.DisplayLine(new string(space, System.Console.BufferWidth));
        System.Console.ResetColor();
    }

    public static Color MakeColorFromHexString(string hexString)
    {
        return ColorTranslator.FromHtml(hexString);
    }
}