using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Console.Utilitys.Url;

public class LocalUrlProvider : ILocalUrlProvider
{
    public string BaseUrl { get; internal set; }

    public bool HasNodeInstalled { get; } = false;

    public NamedPipeClientStream Pipe { get; private set; }

    public event OnPipeDataReceived? OnPipeDataReceived;

    public LocalUrlProvider(IConsole parent)
    {
        // firstly check if node is even installed.
        var (isNodeInstalled, nodePath) = GetNodePath();

        if (!isNodeInstalled)
        {
            parent.Commands.ExecuteFrom(parent, "pkg-install", "nodejs", "--prompt");
        }
    }

    public void Disconnect(string? reason = null)
    {
        throw new NotImplementedException();
    }

    private (bool NodeExists, string? NodePath) GetNodePath()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return GetNodePathMingw();
        }

        throw new NotImplementedException("implement GetNodePath() for gnu based systems.");
    }

    private static (bool, string?) GetNodePathMingw()
    {
        var data = (string?)Registry.GetValue(@"Computer\HKEY_LOCAL_MACHINE\SOFTWARE\Node.js", "InstallPath", null);
        if (data == null)
        {
            return (false, null);
        }
        return (true, data);
    }
}
