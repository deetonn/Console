namespace Console.Commands.Builtins;

public class HelpCommand : BaseBuiltinCommand
{
    public override string Name => "Help";
    public override string Description => "List all active commands.";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, Terminal parent)
    {
        base.Run(args, parent);
        var wantsBuiltin = args.Contains("--builtin");

        if (wantsBuiltin)
        {
            var builtins = parent.Commands.Elements.Where(x => x is BaseBuiltinCommand);
            foreach (var command in builtins)
            {
                parent.Ui.DisplayLine($"{command.Name}: {command.Description}");
            }
            
            return 2;
        }
        
        foreach (var command in parent.Commands.Elements)
        {
            parent.Ui.DisplayLine($"{command.Name}: {command.Description}");
        }

        return 0;
    }
}