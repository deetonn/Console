using Console.Extensions;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http.Handlers;

using Zip = System.IO.Compression.ZipFile;

namespace Console.Commands.Builtins.Web;

public enum InstallerType
{
    /// <summary>
    /// The installer is just an application that needs to be run for user setup.
    /// </summary>
    WindowsExe,
    /// <summary>
    /// The download is a compressed file. It must be extracted and the user needs to
    /// be prompted on where to install the application. The user should also be
    /// asked if it should be added to the PATH variable.
    /// </summary>
    Compressed,

    /// <summary>
    /// A windows ".msi" file. We just need to execute it the same as
    /// an exe.
    /// </summary>
    WindowsMsi,
}

public class PackageData
{
    public string DownloadLink { get; set; }
    public string Description { get; set; }
    public InstallerType Type { get; set; }

    public PackageData(string downloadLink, string description, InstallerType type)
    {
        DownloadLink = downloadLink;
        Description = description;
        Type = type;
    }
}

public class PkgInstall : BaseBuiltinCommand
{
    public static readonly Dictionary<string, PackageData> PackageDirectory = new()
    {
        ["vscode"] =
            new PackageData("https://code.visualstudio.com/sha/download?build=stable&os=win32-x64-user",
                  "Visual Studio Code is a code editor redefined and optimized for building and debugging modern web and cloud applications.",
                  InstallerType.WindowsExe),
        ["vs22"] =
            new PackageData("https://c2rsetup.officeapps.live.com/c2r/downloadVS.aspx?sku=community&channel=Release&version=VS2022&source=VSLandingPage",
                 "Visual Studio design, code with autocompletions, build, debug, test all-in-one place along with Git management and cloud deployments.",
                 InstallerType.WindowsExe),
        ["git-win32"] =
            new PackageData("https://git-scm.com/download/win", "Git is a free and open source distributed version control system designed to handle everything from small to very large projects with speed and efficiency. (for win32)", InstallerType.WindowsExe),
        ["git-unix"] =
            new PackageData("https://git-scm.com/download/linux", "Git is a free and open source distributed version control system designed to handle everything from small to very large projects with speed and efficiency. (for unix based systems)", InstallerType.WindowsExe),
        ["git-mac"] =
            new PackageData("https://git-scm.com/download/mac", "Git is a free and open source distributed version control system designed to handle everything from small to very large projects with speed and efficiency. (for macOS)", InstallerType.WindowsExe),
        ["python"] =
            new PackageData("https://www.python.org/ftp/python/3.11.3/python-3.11.3-amd64.exe", "Python is a high-level, general-purpose programming language.", InstallerType.WindowsExe),
        ["firefox-win"] = 
            new PackageData("https://download.mozilla.org/?product=firefox-stub&os=win&lang=en-GB", "Mozilla Firefox, or simply Firefox, is a free and open-source web browser developed by the Mozilla Foundation and its subsidiary, the Mozilla Corporation.", InstallerType.WindowsExe),
        ["firefox-unix"] = 
            new PackageData("https://download.mozilla.org/?product=firefox-stub&os=linux&lang=en-GB", "Mozilla Firefox, or simply Firefox, is a free and open-source web browser developed by the Mozilla Foundation and its subsidiary, the Mozilla Corporation.", InstallerType.WindowsExe),
        ["firefox-mac"] = 
            new PackageData("https://download.mozilla.org/?product=firefox-stub&os=mac&lang=en-GB", "Mozilla Firefox, or simply Firefox, is a free and open-source web browser developed by the Mozilla Foundation and its subsidiary, the Mozilla Corporation.", InstallerType.WindowsExe),
        ["nano-win64"] = 
            new PackageData("https://github.com/okibcn/nano-for-windows/releases/download/v7.2-22.1/nano-for-windows_win64_v7.2-22.1.zip", "Nano is a lightweight CLI editor. Ported from UNIX to windows.", InstallerType.Compressed),
        ["winrar"] = 
            new PackageData("https://www.win-rar.com/fileadmin/winrar-versions/winrar/winrar-x64-621.exe", "WinRAR is a trialware file archiver utility for Windows, developed by Eugene Roshal of win.rar GmbH. It can create and view archives in RAR or ZIP file formats, and unpack numerous archive file formats.", InstallerType.WindowsExe),
        ["docker"] = 
            new PackageData("https://desktop.docker.com/win/main/amd64/Docker%20Desktop%20Installer.exe?utm_source=docker&utm_medium=webreferral&utm_campaign=dd-smartbutton&utm_location=module", "Docker is a set of platform as a service products that use OS-level virtualization to deliver software in packages called containers.", InstallerType.WindowsExe),
        ["nodejs"] = 
            new PackageData("https://nodejs.org/dist/v18.17.1/node-v18.17.1-x64.msi", "Node.js® is an open-source, cross-platform JavaScript runtime environment.", InstallerType.WindowsMsi)
    };

    public override string Name => "pkg-install";
    public override string Description => "Install an application locally.";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        if (args.Count < 1 || args.Contains("--help"))
        {
            WriteLine($"USAGE: {Name} <package-name>");
            return -1;
        }

        var packageString = args[0];

        // This exists for other parts of Console that want to install packages for
        // full functionality, but want to ask for permission before-hand.
        if (args.Contains("--prompt"))
        {
            var data = ReadLine($"would you like to install \"{packageString}\"? [[Y/n]] ")?.ToLower();
            if (data == null)
            {
                return -4;
            }
            if (data.Contains('n'))
            {
                return -4;
            }
        }

        if (!PackageDirectory.TryGetValue(packageString, out PackageData? value))
        {
            WriteLine($"ERROR: No such package `{packageString}`. Use pkg-list for information.");
            return -1;
        }

        var package = value;

        switch (package.Type)
        {
            case InstallerType.WindowsExe:
                Task.Run(async () => await InstallMsiPackageAsync(packageString, package, parent));
                break;
            case InstallerType.Compressed:
                Task.Run(async () => InstallZipPackageAsync(packageString, package, parent));
                break;
            case InstallerType.WindowsMsi:
                Task.Run(async () => await InstallMsiPackageAsync(packageString, package, parent));
                break;
            default:
                {
                    Debug.Assert(false, $"Unhandled InstallerType ({package.Type})");
                    break;
                }
        }

        return 0;
    }

    public async Task InstallMsiPackageAsync(string packageName, PackageData package, IConsole parent)
    {
        var fileName = $@"C:/Users/{Environment.UserName}/Downloads/_Temp_{packageName}.msi";

        var handler = new HttpClientHandler() { AllowAutoRedirect = true };
        var ph = new ProgressMessageHandler(handler);

        ph.HttpReceiveProgress += (_, args) =>
        {
            parent.Ui.Clear();
            var total = args.TotalBytes;
            var received = args.BytesTransferred;
            var total_str = ToMB(total ?? 0) == 0 ? $"Unknown" : $"{ToMB(total ?? 0)}";
            WriteLine($"{args.ProgressPercentage}% Done | {ToMB(received)}Mb / {total_str}Mb");
            Task.Delay(5).RunSynchronously();
        };

        using var client = new HttpClient(ph);
        parent.Ui.Clear();

        if (File.Exists(fileName))
            File.Delete(fileName);

        await client.DownloadFileTaskAsync(new Uri(package.DownloadLink), fileName);

        Process? installerInstance;

        try
        {
            var info = new ProcessStartInfo(fileName)
            {
                // Run the file as adminstrator, this is generally okay
                // as most of them are installers and request it either way.
                FileName = "msiexec",
                Arguments = $"{fileName}",
                Verb = "runas",
            };
            installerInstance = Process.Start(info);
        }
        // catch relevant exceptions
        catch (Win32Exception ex)
        {
            WriteLine($"failed to start process. {ex.Message} (run as admin/root)");
            return;
        }
        catch (Exception ex)
        {
            WriteLine($"ERROR: {ex.Message}");
            return;
        }
        WriteLine($"Waiting for [{installerInstance?.ProcessName}] to finish.");
        installerInstance?.WaitForExit();

        File.Delete(fileName);
    }

    public async Task InstallZipPackageAsync(string packageName, PackageData package, IConsole parent)
    {
        Logger().LogInfo(this, $"Installing Zip package `{packageName}`.");
        Logger().LogInfo(this, $"^^^ Package Url is `{package.DownloadLink}` ^^^");

        string? path = null;
        do
        {
            var recv = parent.Ui.GetLine($"Where would you like to install `{packageName}`? ");
            if (!Directory.Exists(recv))
            {
                parent.Ui.DisplayLine("\nNo such directory. Please make sure the directory exists.\n");
            }
            else
            {
                path = recv;
            }
        } while (path is null);

        var fullPath = Path.Combine(path, Random.Shared.Next().ToString() + ".zip");
        var handler = new HttpClientHandler() { AllowAutoRedirect = true };
        var ph = new ProgressMessageHandler(handler);

        ph.HttpReceiveProgress += (_, args) =>
        {
            parent.Ui.Clear();
            var total = args.TotalBytes;
            var received = args.BytesTransferred;
            WriteLine($"{args.ProgressPercentage}% Done | {ToMB(received)}Mb / {ToMB(total!.Value)}Mb");
            Task.Delay(5).RunSynchronously();
        };

        using var client = new HttpClient(ph);
        parent.Ui.Clear();

        await client.DownloadFileTaskAsync(new Uri(package.DownloadLink), fullPath);

        WriteLine($"Downloaded to temporary file `{path}`.\n Extracting contents...");

        try
        {
            Zip.ExtractToDirectory(fullPath, path, true);
            File.Delete(fullPath);
        }
        catch (Exception exception)
        {
            WriteLine($"FAILURE: {exception.Message}");
            try
            {
                File.Delete(fullPath);
            } catch { }
            return;
        }

        var pathAnswer = parent.Ui.GetLine("\nWould you like to add this application to the `PATH` variable? (Y/n) ");
        var wantsPath = pathAnswer.ToLower().Contains('y');

        if (!wantsPath)
        {
            WriteLine("\nDone.");
            return;
        }

        try
        {
            Environment.SetEnvironmentVariable("PATH",
        Environment.GetEnvironmentVariable("PATH" + $";{path}"),
        EnvironmentVariableTarget.Machine);

            WriteLine("\nAdded to `PATH` environment variable.\nDone!");
        }
        catch (Exception ex)
        {
            Write($"\nFailed to add to path. ({ex.Message})");
        }
    }

    public static long ToMB(long bytes)
    {
        //     bytes    kb     mb    gb   tb
        return bytes / 1024 / 1024;
    }

    public async Task InstallWindowsExeAsync(string packageName, PackageData package, IConsole parent)
    {
        var fileName = $@"C:/Users/{Environment.UserName}/Downloads/_Temp_{packageName}.exe";

        var handler = new HttpClientHandler() { AllowAutoRedirect = true };
        var ph = new ProgressMessageHandler(handler);

        ph.HttpReceiveProgress += (_, args) =>
        {
            parent.Ui.Clear();
            var total = args.TotalBytes;
            var received = args.BytesTransferred;
            var total_str = ToMB(total ?? 0) == 0 ? $"Unknown" : $"{ToMB(total ?? 0)}";
            WriteLine($"{args.ProgressPercentage}% Done | {ToMB(received)}Mb / {total_str}Mb");
            Task.Delay(5).RunSynchronously();
        };

        using var client = new HttpClient(ph);
        parent.Ui.Clear();

        if (File.Exists(fileName))
            File.Delete(fileName);

        await client.DownloadFileTaskAsync(new Uri(package.DownloadLink), fileName);

        Process? installerInstance;

        try
        {
            var info = new ProcessStartInfo(fileName)
            {
                // Run the file as adminstrator, this is generally okay
                // as most of them are installers and request it either way.
                Verb = "runas",
            };
            installerInstance = Process.Start(info);
        }
        // catch relevant exceptions
        catch (Win32Exception ex)
        {
            WriteLine($"failed to start process. {ex.Message} (run as admin/root)");
            return;
        }
        catch (Exception ex)
        {
            WriteLine($"ERROR: {ex.Message}");
            return;
        }
        WriteLine($"Waiting for [{installerInstance?.ProcessName}] to finish.");
        installerInstance?.WaitForExit();

        File.Delete(fileName);
    }

    public override string DocString => $@"
This command will attempt to install a known package.

USAGE: {Name} <package-name>
    <package-name> - The name of the package to install. Use pkg-list for a list of packages.

NOTE: This command requires an internet connection to download the package. It also
      does connect to a remote address to download the installer for specified applications.
      If you plan on using this, but feel like it may be risky, all download links
      are available in the source code of this application. They are also displayed
      when you run the pkg-list command.
";
}
