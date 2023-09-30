namespace Console.Commands.Scripting;

public delegate void OnSecErr(string message);

public record class CommandExecutionState(ICommand Command, List<string> Args);

public class ScriptExecutionContext
{
    // Stands for Console Script File
    public const string ScriptExtension = ".csf";

    public List<CommandExecutionState> Commands { get; }

    public ScriptExecutionContext(Terminal parent, string file, OnSecErr? err)
    {
        Commands = new List<CommandExecutionState>();

        if (!File.Exists(file))
        {
            err?.Invoke("The specified file does not exist.");
            return;
        }

        var fileInfo = new FileInfo(file);

        if (fileInfo.Extension != ScriptExtension)
        {
            err?.Invoke("The specified file is not a script file.");
            return;
        }

        var lines = File.ReadAllLines(file);
        foreach (var line in lines)
        {
            var command = line.Split(' ')[0];
            var args = line.Split(' ').Skip(1).ToList();

            var cmd = parent.Commands.GetCommand(command);
            if (cmd == null)
            {
                err?.Invoke($"The command `{command}` does not exist.");
                return;
            }

            Commands.Add(new CommandExecutionState(cmd, args));
        }
    }

    public void Execute(Terminal parent)
    {
        foreach (var execState in Commands)
        {
            execState.Command.Run(execState.Args, parent);
        }
    }
}
