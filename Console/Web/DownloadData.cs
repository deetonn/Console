namespace Console.Web;

/// <summary>
/// The version information about the file.
/// </summary>
/// <param name="Major"></param>
/// <param name="Minor"></param>
/// <param name="Patch"></param>
public record class TerminalVersionInfo(
    int Major, int Minor, int Patch
)
{
    public static bool operator >(TerminalVersionInfo left, TerminalVersionInfo right)
    {
        return left.Major > right.Major ||
            left.Minor > right.Minor ||
            left.Patch > right.Patch;
    }

    public static bool operator <(TerminalVersionInfo left, TerminalVersionInfo right)
    {
        return left.Major < right.Major ||
            left.Minor < right.Minor ||
            left.Patch < right.Patch;
    }

    public override string ToString()
    {
        return $"{Major}.{Minor}.{Patch}";
    }
}

public class TerminalData
{
    public required TerminalVersionInfo Version { get; init; }
    public required string? Path { get; init; }

    public static TerminalData ServerOffline =>
        new() { Version = new TerminalVersionInfo(0, 0, 0), Path = null };
}

public record AppNeedsUpdateResponse
    (bool NeedsUpdate, string? DownloadLink);

public record struct DownloadData
    (string BytesEncodedInBase64, uint Count, TerminalVersionInfo Version);
