
using Newtonsoft.Json;
using System.Diagnostics;

namespace Console.Web;

public class Updates {};

public static class AutoUpdater
{
    public const string Url = "https://localhost:7030";
    public static TerminalVersionInfo ThisVersion
        => new(1, 0, 2);

    private static readonly Updates _this = new();

    /// <summary>
    /// This function will check if the current application
    /// has any updates. If it does, it will download the
    /// newest version and execute it, killing this process.
    /// </summary>
    public static void CheckForUpdates(IConsole parent)
    {
        var client = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(2)
        };
        var thisVersion = ThisVersion;
        var url = $"{Url}/AppNeedsUpdate/update/{thisVersion.Major}/{thisVersion.Minor}/{thisVersion.Patch}";

        HttpResponseMessage? response;

        try
        {
            response = client.GetAsync(url).Result;
        }
        catch (Exception e)
        {
            parent.Ui.DisplayLineMarkup($"failed to contact server! ([italic]{e.Message}[/])");
            return;
        }

        var res = response?.Content.ReadAsStringAsync().Result;
        if (res is null)
        {
            Logger().LogWarning(_this, "No response from server about updates.");
            return;
        }
        var data = JsonConvert.DeserializeObject<AppNeedsUpdateResponse>(res);

        if (data == null)
        {
            Logger().LogWarning(_this, "No response from server about updates.");
            return;
        }

        if (data.NeedsUpdate)
        {
            UpdateApplication(parent, Url + data.DownloadLink);
            return; // unlikely return
        }

        parent.Ui.DisplayLineMarkup($"application is up to date!. (local => [italic]{ThisVersion}[/] matches server build)");
    }

    private static void UpdateApplication(IConsole parent, string url)
    {
        var client = new HttpClient();
        HttpResponseMessage? response;

        try
        {
            response = client.GetAsync(url).Result;
        }
        catch (Exception e)
        {
            parent.Ui.DisplayLineMarkup($"failed to contact server! ([italic]{e.Message}[/])");
            return;
        }

        var res = response.Content.ReadAsStringAsync().Result;
        var data = JsonConvert.DeserializeObject<DownloadData>(res);

        var bytes = Convert.FromBase64String(data.BytesEncodedInBase64);
        parent.Ui.DisplayLineMarkup($"downloaded new binary from server. (size = [green]{data.Count}[/])");
        parent.Ui.DisplayLinePure($"Updating to new version ({data.Version}, from {ThisVersion}) in 5s.");

        Thread.Sleep(5000);

        Directory.CreateDirectory("update");
        var cwd = Directory.GetCurrentDirectory();

        var path = Path.Combine(Path.Combine(cwd, "update"), "console.exe");

        File.WriteAllBytes(path, bytes);

        parent.Ui.DisplayLineMarkup($"wrote new binary to `[italic]{path}[/]`");
        // The --update flag tells this process to fix all the path related stuff.
        Process.Start(path, "--update");
    }
}
