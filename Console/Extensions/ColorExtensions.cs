
using System.Drawing;

namespace Console.Extensions;

public static class ColorExtensions
{
    public static string ToHexString(this Color color)
    {
        var red = string.Format("{0:X2}", color.R);
        var green = string.Format("{0:X2}", color.G);
        var blue = string.Format("{0:X2}", color.B);

        return $"#{red}{green}{blue}";
    }
}
