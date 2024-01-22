
using System.Text;

namespace Console.Extensions;

public static class StringExtensions
{
    public static string MarkupStrip(this string text)
    {
        return text.Replace("[", "[[").Replace("]", "]]");
    }

    public static string GetAllBetweenStartingAt(this string self, char thing, ref int position)
    {
        ++position;
        var s = new StringBuilder();

        for (; position < self.Length; position++)
        {
            if (self[position] == thing)
            {
                ++position;
                break;
            }

            s.Append(self[position]);
        }

        return s.ToString();
    }
}
