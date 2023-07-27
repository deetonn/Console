using System.Diagnostics;
using PInvoke;
using Console.Extensions;
using Pastel;
using System.Drawing;

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
    
    //                                ignored `.exe`
    public string Name => _file.Name[..^4];
    
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

    private static Thread LaunchQuitKeyThread(Terminal parent, Process process)
    {
        const int VK_CONTROL = 0x11;

        var t = new Thread(() =>
        {
            while (!process.HasExited)
            {
                parent.Ui.DisplayLine("Thread is spinning!"); 

                bool is_ctrl = (User32.GetAsyncKeyState(VK_CONTROL) & 0x8000) == 0;
                bool is_x = (User32.GetAsyncKeyState(0x58) & 0x8000) == 0;

                if (is_ctrl && is_x)
                {
                    process.Kill();
                }

                Thread.Sleep(1);
            }
        });
        t.Start();
        return t;
    }

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
        string? readData;
        try
        {
            process = PausedInstance ?? Process.Start(startInfo);

            if (process is null)
                return FailedToStartProcess;
            
            process.Resume();
            parent.Ui.DisplayLine($"Process `{process.ProcessName}` has begun! Press Ctrl+X to stop it.");
            readData = process.StandardOutput.ReadToEnd();
            var thread = LaunchQuitKeyThread(parent, process);
            process.WaitForExit();
            thread.Join();
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

    public string DocString => $@"
This command will execute the file at the path `{_file.FullName}`.
This exists due to it being present in a directory included within the PATH environment variable.

If this command appears to be a windows command, try this link:
{$"https://learn.microsoft.com/en-us/windows-server/administration/windows-commands/active/{_file.Name}".Pastel(Color.SkyBlue)}

If that link does not work, search the command on google. 
";
}