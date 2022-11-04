using System.Diagnostics;
using Console.Extensions;

namespace Console.Commands;

public class PathFileCommand : ICommand
{
    public const int FileMovedOrDeleted = -0xDEAD;
    public const int FailedToStartProcess = -0xDEAF;
    
    private readonly FileInfo _file;
    private readonly FileVersionInfo _versionInfo;
    
    public PathFileCommand(FileInfo info)
    {
        _file = info;
        _versionInfo = FileVersionInfo.GetVersionInfo(_file.FullName);
    }
    
    public string Name => _file.Name;
    
    public string Description => _versionInfo.FileDescription ?? "no description";
    
    public DateTime? LastRunTime { get; set; } = DateTime.Now;

    public Process? PausedInstance;

    public bool StartThenPause(List<string> args)
    {
        LastRunTime = DateTime.Now;

        if (!_file.Exists)
        {
            return false;
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = _file.FullName,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            Arguments = string.Join(' ', args),
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            PausedInstance = Process.Start(startInfo);
            PausedInstance?.Suspend();
        }
        catch
        {
            return false;
        }

        return PausedInstance != null;
    }

    public int ResumeExecution(List<string> args, Terminal terminal)
    {
        return Run(args, terminal);
    }

    private Terminal? _terminal = null;

    public int Run(List<string> args, Terminal parent)
    {
        _terminal = parent;
        LastRunTime = DateTime.Now;

        if (!_file.Exists)
        {
            return FileMovedOrDeleted;
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = _file.FullName,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            Arguments = string.Join(' ', args),
            UseShellExecute = false
        };
        
        Process? process;
        string? readData = null;
        
        try
        {
            process = PausedInstance ?? Process.Start(startInfo);

            if (process is null)
                return FailedToStartProcess;
            
            process.Resume();
            parent.Ui.DisplayLine($"Process `{process.ProcessName}` has began!");
            readData = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }
        catch
        {
            return FailedToStartProcess;
        }
        
        parent.Ui.DisplayLinePure(readData);
        return process?.ExitCode ?? -1;
    }

    private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e?.Data is null)
        {
            return;
        }
        
        _terminal?.Ui.DisplayLinePure(e.Data);
    }
}