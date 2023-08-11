namespace Console.Commands.Builtins;

public class LastRanAtCommand : BaseBuiltinCommand
{
    public override string Name => "lastranat";
    public override string Description => "Check the last time a command was ran.";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        if (args.Count != 1)
        {
            parent.Ui.DisplayLine("usage: LastRanAt <command>");
            return -1;
        }

        var name = args[0];
        if (parent.Commands.Elements.All(x => x.Name != name))
        {
            parent.Ui.DisplayLine($"no such command `{name}`");
            return -2;
        }

        var command = parent.Commands.Elements.First(x => x.Name == name);

        if (command.LastRunTime is null)
        {
            parent.Ui.DisplayLine("That command has never been ran!");
            return 0;
        }
        
        var diff = (DateTime.Now -command.LastRunTime);
        var lastRanAt = $"{diff?.Hours} hours, {diff?.Minutes} minutes and {diff?.Seconds} seconds";
        parent.Ui.DisplayLine($"command `{name}` was last ran {lastRanAt} ago");

        return 2;
    }

    public override string DocString => $@"
This command will display the last time a command was ran.

The syntax is as follows:
    {Name} <command-name>

This relies on the command calling `base.Run(args, parent)` in the `Run` method.
Some commands loaded by plugins may not support this, and a garbage time may be displayed.
";
}