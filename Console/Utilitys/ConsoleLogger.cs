namespace Console.Utilitys;

public class ConsoleLogger : ILogger
{
    public void Info(object sender, string message)
    {
        System.Console.WriteLine($"({sender.GetType().Name}) [INFO] {message}");
    }

    public void Err(object sender, string message)
    {
        System.Console.WriteLine($"({sender.GetType().Name}) [ERROR] {message}");
    }
}