using Console.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Http.Handlers;
using static PInvoke.Kernel32;

using Zip = global::System.IO.Compression.ZipFile;

namespace Console.Commands.Builtins.Web;

public enum InstallerType
{
    /// <summary>
    /// The installer is just an application that needs to be run for user setup.
    /// </summary>
    WindowsExe,
    /// <summary>
    /// The download is a .zip file. It must be extracted and the user needs to
    /// be prompted on where to install the application. The user should also be
    /// asked if it should be added to the PATH variable.
    /// </summary>
    Zip
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
            new PackageData("https://github.com/okibcn/nano-for-windows/releases/download/v7.2-22.1/nano-for-windows_win64_v7.2-22.1.zip", "Nano is a lightweight CLI editor. Ported from UNIX to windows.", InstallerType.Zip)
    };

    public override string Name => "pkg-install";
    public override string Description => "Install an application locally.";
    public override DateTime? LastRunTime { get; set; } = null;
    public override int Run(List<string> args, Terminal parent)
    {
        base.Run(args, parent);

        if (args.Count < 1 || args.Contains("--help"))
        {
            WriteLine($"USAGE: {Name} <package-name>");
            return -1;
        }

        var packageString = args[0];

        if (!PackageDirectory.ContainsKey(packageString))
        {
            WriteLine($"ERROR: No such package `{packageString}`. Use pkg-list for information.");
            return -1;
        }

        var package = PackageDirectory[packageString];

        switch (package.Type)
        {
            case InstallerType.WindowsExe:
                InstallWindowsExe(packageString, package, parent);
                break;
            case InstallerType.Zip:
                InstallZipPackage(packageString, package, parent);
                break;
            default:
                {
                    Debug.Assert(false, $"Unhandled InstallerType ({package.Type})");
                    break;
                }
        }

        return 0;
    }

    public void InstallZipPackage(string packageName, PackageData package, Terminal parent)
    {
        string? path = null;
        do
        {
            parent.Ui.Display($"Where would you like to install `{packageName}`? ");
            var recv = parent.Ui.GetLine();
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

        ph.HttpReceiveProgress += async (_, args) =>
        {
            parent.Ui.Clear();
            var total = args.TotalBytes;
            var received = args.BytesTransferred;
            WriteLine($"{args.ProgressPercentage}% Done | {ToMB(received)}Mb / {ToMB(total!.Value)}Mb");
            await Task.Delay(5);
        };

        using var client = new HttpClient(ph);
        parent.Ui.Clear();

        var download = Task.Run(async () =>
        {
            await client.DownloadFileTaskAsync(new Uri(package.DownloadLink), fullPath);
        });
        download.Wait();

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

        Write("\nWould you like to add this application to the `PATH` variable? (Y/n) ");
        var pathAnswer = parent.Ui.GetLine();
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
        return bytes / 1024 / 1024;
    }

    public void InstallWindowsExe(string packageName, PackageData package, Terminal parent)
    {
        var fileName = $@"C:/Users/{Environment.UserName}/Downloads/_Temp_{packageName}.exe";

        var handler = new HttpClientHandler() { AllowAutoRedirect = true };
        var ph = new ProgressMessageHandler(handler);

        ph.HttpReceiveProgress += async (_, args) =>
        {
            parent.Ui.Clear();
            var total = args.TotalBytes;
            var received = args.BytesTransferred;
            WriteLine($"{args.ProgressPercentage}% Done | {ToMB(received)}Mb / {ToMB(total!.Value)}Mb");
            await Task.Delay(5);
        };

        using var client = new HttpClient(ph);
        parent.Ui.Clear();

        var download = Task.Run(async () =>
        {
            await client.DownloadFileTaskAsync(new Uri(package.DownloadLink), fileName);
        });
        download.Wait();

        var installerInstance = Process.Start(fileName);
        WriteLine($"Waiting for [{installerInstance.ProcessName}] to finish.");
        installerInstance.WaitForExit();

        File.Delete(fileName);
    }
}
