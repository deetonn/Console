[![dotnet package](https://github.com/deetonn/Console/actions/workflows/dotnet-desktop.yml/badge.svg?branch=master)](https://github.com/deetonn/Console/actions/workflows/dotnet-desktop.yml) ![GitHub stars](https://img.shields.io/github/stars/deetonn/Console.svg)

# Console 🖥️
![Console logo](https://github.com/deetonn/Console/blob/master/console-logo.png)
## Overview 
 This is a fun and flexible terminal-like application that allows users to interact with the command line interface. The application is designed to be extensible with custom commands and externally loadable plugins, giving users the power to customize their terminal experience. 
## Features  
 - Custom Commands: Users can create and load custom commands by implementing the provided interface. 
 - Flexible Plugins: Externally loadable plugins have access to the entire code base, making them highly versatile. 
 -  Customizable Themes: Users can customize the terminal's appearance with different font colors, background, and prompt style.  
 - Performance focus with extremely fast loading speeds and execution speeds.
 - Command aliasing is supported. The builtin `alias` command uses unix style syntax.
 - Full documentation support for commands. The `docs` command handles this.
 - Inline environment variable support with `{}` syntax. See [this](https://github.com/deetonn/Console/issues/4) for more information.
 ## Installation 
 - Download the latest release from the release tab to the right of the project files.
 - Extract the Zip archive to your location of choice.
 - Optionally, add the directory you chose into the PATH environment variable.
 - If you have Windows Terminal, click on the small arrow that is pointing down next to your recently opened tabs.
 - Select settings
 - On the options on the left, scroll down to the very bottom and select `Add a new profile`. Then select `New empty profile`.
 - Choose a name of your choice, then set the `command line` option to point to the `Console.exe` you just installed.
 - Go back to start-up and set it as your default profile.
 - Profit!
 ## Usage 
 It works just the same as the normal command processor. So use it normally if you wish. 

 You can also use any of the custom builtins of `Console`. Type `help` to view them. To view windows commands, use `help --all`.

 ### Plugins
 - If you want to get started with plugins, get started by creating a new C# project of type `Class Library`.
 - Add a project reference, this needs to point to wherever you installed the `Console` application. The `.dll` file you want to include is `Console.dll`.
 - Create a class and name it however you like, then implement the interface in `Console.Plugins.IConsolePlugin`.
 - Below is an example of how you must implement the class.
```cs
public class ExamplePlugin : Console.Plugins.IConsolePlugin
{
    // Doesn't have to by `_` seperated.
    public string Name => "my_plugin_name";

    public string Description => "My description";

    public string Author => "Your name";

    public Guid Id { get; set; } = Guid.Empty; // This is set by the plugin manager.

    public void OnCommandExecuted(Terminal terminal, ICommand command)
    {
        throw new NotImplementedException();
    }

    public void OnLoaded(Terminal terminal)
    {
        throw new NotImplementedException();
    }

    public void OnSettingChange(Terminal terminal, ISettings settings, string settingName, object newValue)
    {
        throw new NotImplementedException();
    }

    public void OnUnloaded(Terminal terminal)
    {
        throw new NotImplementedException();
    }

    public bool OnUserInput(Terminal terminal, string input)
    {
        throw new NotImplementedException();
    }
}
```
- Build the application.
- If you have finished your plugin (or installing someone elses) you can find the auto-loaded plugin path at C:\Users\YourUserName\AppData\Roaming\Console\saved\plugins.
- If you want to test it, copy the path to your plugin, then run the command `load_plugin <path>`.
To print to the console, use the `terminal` arguments member `WriteLine`.
 
 ## Contributing 
 Contributions to this project are welcome! If you find any bugs or have suggestions for new features, feel free to open an issue or submit a pull request. 
 ## License 
 This project is licensed under the [MIT License](https://github.com/deetonn/Console/blob/master/LICENSE).  
 ## Contact 
 For any questions or inquiries, you can reach me in the issues section.
