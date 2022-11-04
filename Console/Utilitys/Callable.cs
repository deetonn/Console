using System.Runtime.Remoting;

namespace Console.Utilitys;

public class Callable
{
    private dynamic Invokable;
    
    public Callable(dynamic callable)
    {
        var type = (Type)callable.GetType();

        if (type is null)
        {
            throw new ArgumentException("failed to get type information of callable");
        }

        var methods = type.GetMethods();
        var hasCall = methods.Any(x => x.Name == "Invoke");

        if (!hasCall)
        {
            throw new ArgumentException("argument is not callable. Must have the `Invoke` method.");
        }

        Invokable = callable;
    }

    public dynamic Function
        => Invokable;
}