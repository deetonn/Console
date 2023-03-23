// See https://aka.ms/new-console-template for more information

using Console;
using Console.UserInterface.UiTypes;
using Console.Utilitys;

Singleton<ILogger>.InitTo(instance: new ConsoleLogger());

var wantsUi = args.Contains(value: "--gui");

var terminal = new Terminal(
    type: wantsUi ? UiType.ImGui : UiType.Console
);

terminal.MainLoop();