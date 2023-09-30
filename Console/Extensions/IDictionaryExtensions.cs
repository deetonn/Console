
namespace Console.Extensions;

public static class DictionaryExtensions
{
    public static void ForEach<T1, T2>(this Dictionary<T1, T2> dict, Action<T1, T2> func)
        where T1 : notnull
    {
        foreach (var (k, v) in dict)
        {
            func(k, v);
        }
    }
}
