[![dotnet package](https://github.com/deetonn/Console/actions/workflows/dotnet-desktop.yml/badge.svg?branch=master)](https://github.com/deetonn/Console/actions/workflows/dotnet-desktop.yml) ![GitHub stars](https://img.shields.io/github/stars/deetonn/Console.svg)

<p align="center">
 <img src="https://github.com/deetonn/Console/blob/master/console-logo.png" height="200" alt="Console logo" />
</p>

# Console 🖥️
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
 ### ⚠️ WARNING
 It does not yet have full compatability with the windows command processor.

 You can also use any of the custom builtins of `Console`. Type `help` to view them. To view windows commands, use `help --all`.

 ## Documentation
 Visit the [wiki](https://github.com/deetonn/Console/wiki) here on github itself!
 
 ## Contributing 
 Contributions to this project are welcome! If you find any bugs or have suggestions for new features, feel free to open an issue or submit a pull request. 
 ## License 
 This project is licensed under the [MIT License](https://github.com/deetonn/Console/blob/master/LICENSE).  
 ## Contact 
 For any questions or inquiries, you can reach me in the issues section.
