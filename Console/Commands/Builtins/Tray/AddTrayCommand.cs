namespace Console.Commands.Builtins;

public class AddTrayCommand : BaseBuiltinCommand
{
    public override string Name => "addtray";
    public override string Description => "Add a queued message";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, Terminal parent)
    {
        base.Run(args, parent);

        if (args.Count < 1)
        {
            // invalid arguments
            WriteLine("usage: AddTray <message>");
            return -1;
        }

        var message = string.Join(' ', args);
        parent.Ui.Tray.AddMessage(message);
        parent.Ui.SetTitle("Added 1 new message!");
        
        return 0;
    }
}