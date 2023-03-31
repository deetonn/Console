
using Console.UserInterface;
using Console.Utilitys.Options;

namespace Console.Commands.Builtins.Config;

public class ReloadConfigCommand : BaseBuiltinCommand
{
    public List<string> RandomPhrases = new List<string>()
    {
        "Tabacco",
        "Chair",
        "Monitor",
        "Drawing",
        "Xbox",
        "Microphone",
        "VapeJuice",
        "Computer",
        "iPhone",
        "Lighter",
        "Msi Optix MAG241C",
        "144hz Monitor",
        "Oasis",
        "Electricity"
    };

    public override string Name => "optreset";
    public override string Description => "Reset the configuration to its defaults";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, Terminal parent)
    {
        base.Run(args, parent);

        WriteLine("You are attempting to reset your configuration!");
        WriteLine($"This operation will delete the saved config in '{Terminal.SavePath}'. Back this up!");
        WriteLine("Please enter the random phrase shown below to confirm the reset.");

        var randomWord = RandomPhrases[Random.Shared.Next(0, RandomPhrases.Count)];

        parent.Ui.DisplayLinePure($"Phrase: {randomWord}");
        var input = parent.Ui.GetLine();

        if (input != randomWord)
        {
            WriteLine("Words do not match!");
            return -1;
        }

        // delete the configuration file.
        File.Delete(Terminal.SavePath);
        parent.Settings = new ConsoleOptions(Terminal.SavePath);

        WriteLine("Reset your configuration!");

        return 0;
    }
}
