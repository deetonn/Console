
using System.IO.Pipes;

namespace Console.Utilitys.Url;

public record class ExecutionRequest(string Command, List<string> Arguments);

public enum ActionType
{
    /// <summary>
    /// If the data is of this type, this is an execute request.
    /// The data should be casted to <see cref="ExecutionRequest"/>
    /// </summary>
    Execute,
}

public delegate void OnPipeDataReceived(ActionType action, object obj);

public interface ILocalUrlProvider
{
    /// <summary>
    /// The pipe that is talking to the node process. This process is responsible
    /// for actually running the web server.
    /// </summary>
    public NamedPipeClientStream Pipe { get; }

    /// <summary>
    /// The base URL to make requests to. This contains the local ip address,
    /// (localhost) and the port. Example: http://127.0.0.1:0000
    /// </summary>
    public string BaseUrl { get; }

    /// <summary>
    /// Is node installed on the local machine.
    /// </summary>
    public bool HasNodeInstalled { get; }

    /// <summary>
    /// This function will be called whenever the client receives information
    /// from the server.
    /// </summary>
    public event OnPipeDataReceived? OnPipeDataReceived;

    /// <summary>
    /// Disconnect from the named pipe. 
    /// </summary>
    /// <param name="reason">The reason, or null if there isn't one.</param>
    public void Disconnect(string? reason = null);
}
