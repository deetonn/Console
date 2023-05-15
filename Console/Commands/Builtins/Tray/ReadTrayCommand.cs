namespace Console.Commands.Builtins;

public class ReadTrayCommand : BaseBuiltinCommand
{
    public override string Name => "tray";
    public override string Description => "Read queued messages";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, Terminal parent)
    {
        base.Run(args, parent);

        if (parent.Ui.Tray.Messages.Count == 0)
        {
            WriteLine("No queued messages.");
            return 1;
        }
        
        if (args.Contains("--skip-to-front"))
        {
            var last = parent.Ui.Tray.Messages[^1];
            var count = parent.Ui.Tray.Messages.Count;
            parent.Ui.Tray.ClearMessages();
            
            WriteLine($"Cleared message queue! [{count} removed]\n");
            WriteLine($"Message: {last}");
            return 2;
        }

        var next = parent.Ui.Tray.Messages.First();
        parent.Ui.Tray.Messages.Remove(next);
        
        WriteLine($"Message: {next}");

        return 0;
    }
}