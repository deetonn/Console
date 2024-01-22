using Console.Errors;
using Console.Extensions;
using Pastel;
using PInvoke;
using System.Diagnostics;
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

    public string Name
    {
        get
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                //                         ignored `.exe`
                return _file.Name[..^4];

            return _file.Name;
        }
    }

    public string Description => _versionInfo.FileDescription ?? "no description";

    public DateTime? LastRunTime { get; set; } = DateTime.Now;

    private IConsole? _terminal = null;
    private Process? _process;

    public Process? GetProcess()
    {
        return _process;
    }

    public CommandResult Run(List<string> args, IConsole parent)
    {
        _terminal = parent;
        LastRunTime = DateTime.Now;

        if (!_file.Exists)
        {
            return new CommandErrorBuilder()
                .WithSource(_terminal?.GetLastExecutedString() ?? "<unavailable>")
                .WithMessage("the file binded to this command name no longer exists.")
                .Build();
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = _file.FullName,
            Arguments = string.Join(' ', args),
            UseShellExecute = false
        };

        Process? process;
        try
        {
            process = _process = Process.Start(startInfo);
            process?.WaitForExit();
        }
        catch (Exception e)
        {
            return new CommandErrorBuilder()
                .WithSource(_terminal?.GetLastExecutedString() ?? "<unavailable>")
                .WithMessage("failed to launch the process.")
                .WithNote("an exception occured while trying to start the process binded to that command name.")
                .WithNote($"error: {e.Message}")
                .Build();
        }

        return process?.ExitCode ?? -1;
    }

    public string DocString => $@"
This command will execute the file at the path `{_file.FullName}`.
This exists due to it being present in a directory included within the PATH environment variable.

If this command appears to be a windows command, try this link:
{$"https://learn.microsoft.com/en-us/windows-server/administration/windows-commands/active/{Name}".Pastel(Color.SkyBlue)}

If that link does not work, search the command on google. 
";
}