// See https://aka.ms/new-console-template for more information

using Console;
using Console.Extensions;
using Console.UserInterface.UiTypes;
using Console.Utilitys;
using Pastel;
using System.Drawing;

Singleton<ILogger>.InitTo(new ConsoleLogger());

System.Console.ReadLine();

var wantsUi = args.Contains("--gui");

var terminal = new Terminal(
    wantsUi ? UiType.ImGui : UiType.Console
);

terminal.MainLoop();