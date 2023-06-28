
using CommandLine;
using System.Formats.Tar;

namespace Console.Commands.Builtins.Web.WebServer;

public class ServerArguments
{
    [Option('P', "password", Required = false, HelpText = "The password to use for the server.")]
    public string? Password { get; set; }

    [Option('p', "port", Required = false, HelpText = "The port to use for the server.")]
    public uint Port { get; set; } = 8080;

    [Option('S', "stop", Required = false, HelpText = "Stop the current server instance.")]
    public bool Stop { get; set; }
}

public class StartServer : BaseBuiltinCommand
{
    public override string Name => "chatsv";
    public override string Description => "Launch a server to talk to people over the network! (not encrypted)";
    public override DateTime? LastRunTime { get => base.LastRunTime; set => base.LastRunTime = value; }

    public override int Run(List<string> args, Terminal parent)
    {
        base.Run(args, parent);

        var parsedArgs = Parser.Default.ParseArguments<ServerArguments>(args);

        if (parsedArgs.Errors.Any())
        {
            parsedArgs.Errors.ToList().ForEach(error => parent.Ui.DisplayLine(error.ToString()!));
            return -1;
        }

        var config = parsedArgs.Value;

        if (config.Stop)
        {
            parent.Server?.Stop();
            WriteLine("Server has been stopped.");
            return 0;
        }

        if (parent.Server is not null)
        {
            WriteLine("A server is already running.");
            return -1;
        }

        var server = new ChatServer(config.Password, parent);
        server.Start((int)config.Port);

        parent.Server = server;
        server.InContext = true;

        WriteLine(string.Format("Server started on {0}:{1} with password `{2}`",
            server.ServerIp(), config.Port, config.Password ?? "No password"));

        return 0;
    }
}
