
using dezi;
using dezi.UiElements;

namespace Console.Commands.Builtins.Etc.Editor;

public class ConsoleEditorCommand : BaseBuiltinCommand
{
    public override string Name => "ced";

    public override string Description => "Edit a file.";

    public override int Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        if (args.Count != 1)
        {
            WriteLine("no file path specified.");
            return -1;
        }

        var path = Path.IsPathRooted(args[0])
            ? args[0] 
            : Path.Combine(parent.WorkingDirectory, args[0]);

        FileInfo fileInfo;
        if (!(fileInfo = new FileInfo(path)).Exists)
        {
            WriteLine($"could not find file \"{fileInfo.FullName}\".");
            return -1;
        }

        var editor = new Ui(new List<string> { fileInfo.FullName });
        editor.Run();

        return 0;
    }
}
