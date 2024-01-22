
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Console.Extensions;

public static class IConsoleExtensions
{
    [DllImport("libc")]
    private static extern uint getuid();

    public static bool IsAdministrator(this IConsole _)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        return getuid() != 0;
    }

    public static void RequestAdminPermissions(this IConsole self, string commandToJumpTo, List<string> arguments, IConsole parent)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = Path.Combine(parent.GetExecutableLocation(), "Console.exe"),
            // "--jump=run --jump-args=wmic diskdrive get serialnumber"
            Arguments = $"--jump={commandToJumpTo} --jump-args={string.Join(", ", arguments)}",
            // require administrator
            Verb = "runas",
            UseShellExecute = false,
        };

        var process = Process.Start(startInfo);
        if (process == null)
        {
            self.Ui.DisplayLineMarkup("[red italic]failed[/] to start process as administrator.");
        }
        process?.WaitForExit();
    }
}
