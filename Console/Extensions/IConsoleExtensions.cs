
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Console.Extensions;

public static class IConsoleExtensions
{
    public static bool IsAdministrator(this IConsole _)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        throw new NotImplementedException("check for root on gnu machines");
    }

    public static void RequestAdminPermissions(this IConsole self, string commandToJumpTo, List<string> arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = Path.Combine(Directory.GetCurrentDirectory(), "Console.exe"),
            // "--jump=run --jump-args=wmic diskdrive get serialnumber"
            Arguments = $"--jump={commandToJumpTo} --jump-args={string.Join(", ", arguments)}",
            // require administrator
            Verb = "runas",

            RedirectStandardOutput = true,
            RedirectStandardError = true,
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
