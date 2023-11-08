
using System.Drawing;

namespace Console.Extensions;

public static class ColorExtensions
{
    public static string ToHexString(this Color color)
    {
        return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
    }

    public static ConsoleColor ClosestConsoleColor(this Color color)
    {
        byte r = color.R, g = color.G, b = color.B;
        ConsoleColor ret = 0;
        double rr = r, gg = g, bb = b, delta = double.MaxValue;

        foreach (ConsoleColor cc in Enum.GetValues(typeof(ConsoleColor)))
        {
            var n = Enum.GetName(typeof(ConsoleColor), cc);
            var c = Color.FromName(n == "DarkYellow" ? "Orange" : n ?? "White"); // bug fix
            var t = Math.Pow(c.R - rr, 2.0) + Math.Pow(c.G - gg, 2.0) + Math.Pow(c.B - bb, 2.0);
            if (t == 0.0)
                return cc;
            if (t < delta)
            {
                delta = t;
                ret = cc;
            }
        }
        return ret;
    }
}
