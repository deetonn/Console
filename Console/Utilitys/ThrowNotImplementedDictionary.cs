using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Console.Utilitys;

public class ThrowNotImplementedDictionary<T1, T2> : IDictionary<T1, T2>
{
    public T2 this[T1 key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public ICollection<T1> Keys => throw new NotImplementedException();

    public ICollection<T2> Values => throw new NotImplementedException();

    public int Count => throw new NotImplementedException();

    public bool IsReadOnly => throw new NotImplementedException();

    public void Add(T1 key, T2 value)
    {
        throw new NotImplementedException();
    }

    public void Add(KeyValuePair<T1, T2> item)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public bool Contains(KeyValuePair<T1, T2> item)
    {
        throw new NotImplementedException();
    }

    public bool ContainsKey(T1 key)
    {
        throw new NotImplementedException();
    }

    public void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public bool Remove(T1 key)
    {
        throw new NotImplementedException();
    }

    public bool Remove(KeyValuePair<T1, T2> item)
    {
        throw new NotImplementedException();
    }

    public bool TryGetValue(T1 key, [MaybeNullWhen(false)] out T2 value)
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}
