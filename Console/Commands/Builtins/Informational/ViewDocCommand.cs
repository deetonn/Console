
namespace Console.Commands.Builtins.Informational;

public class ViewDocCommand : BaseBuiltinCommand
{
    public override string Name => "docs";
    public override string Description => "View documentation for a command.";

    public override int Run(List<string> args, Terminal parent)
    {
        base.Run(args, parent);

        if (args.Count == 0)
        {
            WriteLine($"{Name}: expected one argument. (The name of the command to view the docs of)");
            return -1;
        }

        var name = args[0];

        var command = parent.Commands.GetCommand(name);

        if (command == null)
        {
            WriteLine($"{Name}: command '{name}' does not exist.");
            return -1;
        }

        WriteLine(command.DocString);
        return 0;
    }

    public override string DocString => $@"
This command will display documentation about a certain command.

Example:
    {Name} cd
";
}
