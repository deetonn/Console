using Console.UserInterface.UiTypes;

namespace Console.UserInterface;

public static class Ui
{
    public static IUserInterface Create(UiType type, Terminal parent)
    {
        return type switch
        {
            UiType.Console => new NativeConsoleUi(parent),
            UiType.ImGui => throw new NotImplementedException("implement ImGui user interface"),
            _ => throw new NotImplementedException($"implement {nameof(UiType)}.{type}")
        };
    }
}