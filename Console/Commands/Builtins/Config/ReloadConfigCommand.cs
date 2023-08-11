
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
    public override int Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        WriteLine("You are attempting to reset your configuration!");
        WriteLine($"This operation will delete the saved config in '{parent.GetConfigPath()}'. Back this up!");
        WriteLine("Please enter the random phrase shown below to confirm the reset.");

        var randomWord = RandomPhrases[Random.Shared.Next(0, RandomPhrases.Count)];

        var input = parent.Ui.GetLine($"Phrase: {randomWord}");

        if (input != randomWord)
        {
            WriteLine("Words do not match!");
            return -1;
        }

        // delete the configuration file.
        File.Delete(parent.GetConfigPath());
        parent.Settings = new ConsoleOptions(parent.GetConfigPath(), parent);

        WriteLine("Reset your configuration!");

        return 0;
    }

    public override string DocString => $@"
This command will reset your configuration to its defaults.

It requires you to enter a random phrase to confirm the reset. This is to protect
you from plugins trying to automatically destroy your config file.

This command will delete the configuration file and re-new it with its defaults.
";
}
