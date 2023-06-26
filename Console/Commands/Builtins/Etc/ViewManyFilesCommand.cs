using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console.Commands.Builtins.Etc;

public class ViewManyFilesCommand : BaseBuiltinCommand
{
    public override string Name => "vwmf";
    public override string Description => "View each code file in a directory.";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, Terminal parent)
    {
        base.Run(args, parent);

        var cwd = args.Any()
            ? args[0] 
            : parent.WorkingDirectory;

        if (!Directory.Exists(cwd))
        {
            WriteLine($"ERROR: incorrect directory input. ({cwd})");
            return CommandReturnValues.DontShowText;
        }

        var files = Directory.GetFiles(cwd, "*", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            _ = parent.Commands.Run(
                "vwf",
                new List<string>()
                {
                    file
                },
                parent);
            WriteLine("Press any key to go next...");
            _ = parent.Ui.GetKey();
        }

        return CommandReturnValues.DontShowText;
    }
}
