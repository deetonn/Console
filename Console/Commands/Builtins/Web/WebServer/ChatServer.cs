
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Console.Commands.Builtins.Web.WebServer;

public delegate void OnMessageReceived(Message message);

public record class InitialPacket
{
    public string? UserName { get; set; }
    public string? Key { get; set; }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public static InitialPacket? FromJson(string json)
    {
        return JsonSerializer.Deserialize<InitialPacket>(json);
    }
}

public class ChatServer : IServer
{
    private bool _isInContext;

    /// <summary>
    /// This tells the server if it's in context,
    /// meaning should it display messages to the user.
    /// </summary>
    public bool InContext 
    { 
        get 
        {
            return _isInContext;
        }
        set 
        {
            _isInContext = value;
            if (value)
            {
                // If the value is true, clear the display and
                // display all the messages.
                Parent.Ui.Clear();
                foreach (var message in Messages)
                {
                    Parent.Ui.DisplayLinePure(message.ToString());
                }
            }
            else
            {
                Parent.Ui.Clear();
            }
        } 
    }

    public IConsole Parent { get; set; }
    public bool NetworkThreadSpinning { get; private set; }

    public TcpListener? Listener { get; set; }
    public List<Message> Messages { get; set; }
    public Dictionary<InitialPacket, TcpClient> Clients { get; set; }
    public string? Password { get; set; }

    private Thread? _networkThread;

    // event for when a message is received.
    public event OnMessageReceived? MessageReceived;

    public string ServerIp()
    {
        return (Listener!.Server.LocalEndPoint as IPEndPoint)!.ToString();
    }

    public ChatServer(string? password, IConsole parent)
    {
        Messages = new List<Message>
        {
            new Message(
            "Server",
            DateTime.Now,
            "Chat server has been initialized!"
            )
        };

        Clients = new();
        Password = password;
        Parent = parent;

        MessageReceived += (message) =>
        {
            if (_isInContext)
            {
                Parent.Ui.DisplayLinePure(message.ToString());
            }
            else
            {
                // Update the title to show the user that there are new messages.
                Parent.Ui.SetTitle($"Chat Server ({Messages.Count} new messages)");
            }
        };
    }

    [Obsolete("Messages are handled by events.", true)]
    public string Receive()
    {
        throw new NotImplementedException();
    }

    public void Send(string data)
    {
        // Send data to the client in the form of a Message.
        var message = new Message("Server", DateTime.Now, data);
        var serialized = message.ToJson();

        foreach (var client in Clients)
        {
            var stream = client.Value.GetStream();
            var buffer = Encoding.ASCII.GetBytes(serialized);
            stream.Write(buffer, 0, buffer.Length);
        }
    }

    public void Start(int port)
    {
        // Initialize the TcpListener on the real IP address.
        Listener = new TcpListener(IPAddress.Any, port);
        Listener.Start();

        NetworkThreadSpinning = true;

        _networkThread = new(() =>
        {
            Logger().LogInfo(this, "Networking thread started for chat server");

            while (NetworkThreadSpinning)
            {
                TcpClient client;           
                try
                {
                     client = Listener.AcceptTcpClient();
                }
                catch
                {
                    continue;
                }
                var stream = client.GetStream();

                // Expect there to be an inital packet,
                // That contains a username & an option key.
                // Do this by reading the data from the client
                // into a string

                var buffer = new byte[1024];
                var data = new StringBuilder();
                var bytes = 0;
                while (bytes < buffer.Length)
                {
                    bytes = stream.Read(buffer, 0, buffer.Length);
                    data.Append(Encoding.ASCII.GetString(buffer, 0, bytes));
                }

                var initialPacket = InitialPacket.FromJson(data.ToString());

                if (initialPacket is null)
                {
                    AddServerMessage("client failed to send a valid first packet.");
                    continue;
                }

                // Make sure the key matches this chat servers key.
                if (initialPacket.Key is null || initialPacket.Key != Password)
                {
                    AddServerMessage($"Someone attempted to connect with the wrong passphrase. Their username was `{initialPacket.UserName}`");
                    continue;
                }
                // add the client into the list of clients.
                Clients.Add(initialPacket, client);
            }
        });

        _networkThread.Start();
    }

    public void Stop()
    {
        // Stop the server
        NetworkThreadSpinning = false;
        Listener?.Stop();
        _networkThread?.Join();
        Logger().LogInfo(this, "Server has been stopped.");
    }

    private void AddServerMessage(string data)
    {
        var msg = new Message(
            "Server",
            DateTime.Now,
            data);
        Messages.Add(msg);
        MessageReceived?.Invoke(msg);
    }
}
