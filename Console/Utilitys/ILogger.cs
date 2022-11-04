namespace Console.Utilitys;

public interface ILogger
{
    public void Info(object sender, string message);
    public void Err(object sender, string message);
}