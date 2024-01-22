
using System.Diagnostics;

if (args.Length != 3)
{
    Console.Error.WriteLine("ERROR: USAGE: Elevate.exe <PATH> <PID>");
    return;
}

var path = args[1];
if (!File.Exists(path))
{
    Console.Error.WriteLine("ERROR: That file does not exist.");
    return;
}

if (!int.TryParse(args[2], out var pid))
{
    Console.Error.WriteLine("Invalid PID.");
    return;
}

var caller = Process.GetProcesses().Where(x => x.Id == pid);

if (caller.Any())
{
    var process = caller.First();
    process.Kill();
}

var startInfo = new ProcessStartInfo
{
    FileName = path,
    Verb = "runas"
};

var application = Process.Start(startInfo);

if (application is null)
{
    Console.Error.WriteLine("ERROR: failed to start process.");
    return;
}

// Run it as a child to this process.
application.WaitForExit();

