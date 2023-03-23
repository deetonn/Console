// See https://aka.ms/new-console-template for more information

using Console;
using Console.UserInterface.UiTypes;
using Console.Utilitys;

Singleton<ILogger>.InitTo(new ConsoleLogger());

var wantsUi = args.Contains("--gui");

var terminal = new Terminal(
    wantsUi ? UiType.ImGui : UiType.Console
);

terminal.MainLoop();