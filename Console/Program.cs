// See https://aka.ms/new-console-template for more information

using Console;
using Console.UserInterface.UiTypes;
using Console.Utilitys;

Singleton<ILogger>.InitTo(instance: new ConsoleLogger());

var wantsUi = args.Contains(value: "--gui");

foreach (var arg in args)
{
    System.Console.WriteLine($"{arg}");
}

var terminal = new Terminal(
    type: wantsUi ? UiType.ImGui : UiType.Console
);

terminal.MainLoop();