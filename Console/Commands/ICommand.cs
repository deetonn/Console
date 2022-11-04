namespace Console.Commands;

public interface ICommand
{
    public string Name { get; }
    public string Description { get; }
    public DateTime? LastRunTime { get; set; }

    public int Run(List<string> args, Terminal parent);
}