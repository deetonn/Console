
using System.Net.Sockets;
using System.Text.Json;

namespace Console.Commands.Builtins.Web.WebServer;

// This will be an interface for a web server controlled
// by a user over a commandline interface.
// The server will be P2P.

// This class will be used to store the connection
public record class Message
(
    string? Sender,
    DateTime Time,
    string Content
)
{
    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public static Message? FromJson(string json)
    {
        return JsonSerializer.Deserialize<Message>(json);
    }

    public override string ToString()
    {
        return $"{Time.ToShortTimeString()} [{Sender}]: {Content}";
    }
}

public interface IServer
{
    public TcpListener? Listener { get; set; }
    public List<Message> Messages { get; set; }

    // The server will be able to start and stop.
    // Only port, due to the IP being deduced to the users
    // ip address.
    public void Start(int port);
    public void Stop();

    // The server will be able to send and receive data.
    public void Send(string data);
    public string Receive();
}
