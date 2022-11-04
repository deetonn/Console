namespace Console.Utilitys;

public class Singleton<T>
{
    private static T? _sInstance;
    private static readonly object _sLock = new();

    public static T Instance()
    {
        lock (_sLock)
        {
            if (_sInstance is null)
            {
                throw new NullReferenceException("must call InitTo on a singleton.");
            }
            return _sInstance;
        }
    }

    public static void InitTo(T instance)
    {
        lock (_sLock)
            _sInstance = instance;
    }
}