namespace Console.Commands.Builtins;

public class AddTrayCommand : BaseBuiltinCommand
{
    public override string Name => "addtray";
    public override string Description => "Add a queued message";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, IConsole parent)
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

    public override string DocString => $@"
This command will add a message into the builtin message tray.

The syntax is as follows:
    {Name} <message>

If the message is empty, the command will display a message notifying you of that
and return a non-zero exit code.

The message does not have to adhere to any specific format, but it is recommended
that you keep it short and sweet.

All arguments will be joined together with a space, so you don't need to use quotes to
specify a message with spaces in it.
";
}