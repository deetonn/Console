using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Console.Extensions;

public static class ProcessExtension
{
    [Flags]
    public enum ThreadAccess : int
    {
        // ReSharper disable once InconsistentNaming
        TERMINATE = (0x0001),
        // ReSharper disable once InconsistentNaming
        SUSPEND_RESUME = (0x0002),
        // ReSharper disable once InconsistentNaming
        GET_CONTEXT = (0x0008),
        // ReSharper disable once InconsistentNaming
        SET_CONTEXT = (0x0010),
        // ReSharper disable once InconsistentNaming
        SET_INFORMATION = (0x0020),
        // ReSharper disable once InconsistentNaming
        QUERY_INFORMATION = (0x0040),
        // ReSharper disable once InconsistentNaming
        SET_THREAD_TOKEN = (0x0080),
        // ReSharper disable once InconsistentNaming
        IMPERSONATE = (0x0100),
        // ReSharper disable once InconsistentNaming
        DIRECT_IMPERSONATION = (0x0200)
    }
    
    [DllImport("kernel32.dll")]
    private static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
    [DllImport("kernel32.dll")]
    private static extern uint SuspendThread(IntPtr hThread);
    [DllImport("kernel32.dll")]
    private static extern int ResumeThread(IntPtr hThread);

    public static void Suspend(this Process process)
    {
        foreach (ProcessThread thread in process.Threads)
        {
            var pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
            if (pOpenThread == IntPtr.Zero)
            {
                break;
            }
            _ = SuspendThread(pOpenThread);
        }
    }
    public static void Resume(this Process process)
    {
        foreach (ProcessThread thread in process.Threads)
        {
            var pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
            if (pOpenThread == IntPtr.Zero)
            {
                break;
            }
            _ = ResumeThread(pOpenThread);
        }
    }
}