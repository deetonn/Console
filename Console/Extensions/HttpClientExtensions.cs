using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console.Extensions;

public static class HttpClientUtils
{
    public static async Task DownloadFileTaskAsync(this HttpClient client, Uri uri, string FileName)
    {
        Logger().LogInfo(client, $"File is being downloaded from `{uri}` into destination path `{FileName}`");

        using var s = await client.GetStreamAsync(uri);
        Logger().LogInfo(client, $"HttpStream established");
        using var fs = new FileStream(FileName, FileMode.CreateNew);
        Logger().LogInfo(client, "FileStream established for target file to write contents.");
        await s.CopyToAsync(fs);
    }
}
