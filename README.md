# Console
A fun project that is a stand-in for the default windows command processor.
The basic windows command processor is garbage. This application steps in and adds *ALOT* more
functionality including the same functionality. 
# Commands

## Configuration
This console contains an entirely native configuration system. This includes changing the colors of certain
colors within the interface, or adding blocks between outputs.
### Related Commands
**optedit** (*EditOptionCommand.cs*) - This command accepts 2 arguments. The name of the option and the new value. This commands accepts hexadecimal format for numbers, true/false & 0/1 and any number.
**Example: `optedit ui.color.username #FF0000`**

**optview** (*ViewOptionsCommand.cs*) - This command accepts no arguments. This will list all of the active/valid configuration options including their current value.

**optreset** (*ReloadConfigCommand.cs*) - This command accepts no arguments. This will reset your entire configuration. Once running the command, you will be warned about the fact that this operation is irreversible.
You will then be asked to enter a random phrase. This exists as a partial security measure.

**optrm** (*RemoveOptionCommand.cs*) - This command accepts one argument. This will remove whichever option is specified. If the option exists, it will be removed. Otherwise, an error message will be displayed.

## Directory Based Commands
This console has the usual directory behavior.
### Related Commands
**cd** (*ChangeDirectoryCommand.cs*) - This command accepts one argument. The argument is the directory to navigate to. If the supplied argument is a relative directory, any changes will be based from the current working directory. If the supplied argument is rooted, then the change will ignore the current working directory. The argument supports all directory syntax. Supported operations include the following:
Example #1: `cd ~` - will change the current directory to the active users profile.
Example #2: `cd ../..` - will navigate back two directory's relative to the current one.
Example #3: All commands accept `.` as a directory (meaning the current directory.) This will cause that command accepting a directory argument to use the current working directory. 

**copy** (*CopyCommand.cs*) - This command will copy a file/directory into another directory.
**Example: `copy Directory1 Directory2`** - Will copy `Directory1` and all of it's children directory's into `Directory2`.

**dir** (*DirCommand.cs*) - This command will display all files and directory within the current working directory along with relevant information about the file/directory.

**linec** (*LineCountCommand.cs*) - This command will count the number of lines within a directory/file. This command is very configurable.
#### `linec` Arguments
`-V`, `--valid-exts` - All file extensions specified (without the `.`) will be processed. Anything else is ignored.
`-d`, `--directory` - The directory to process files in.
`-r`, `--recursive` - If this value is supplied, all child directory's of the specified `-d` directory will be processed for valid files too.
`-v`, `--verbose` - If supplied, information about how many files are left/have already been processed will be displayed, along with the current file being processed.
`-f`, `--file-name` - If supplied, `-d` is ignored. This file will be the only thing processed.
`-P`, `--preset` - Language presets. Valid presets are cpp, c, cs, py, rs. If any of these are chosen, relevant valid file extensions are automatically loaded, verbose is set, recursive is set and the directory is set to the current working directory. This option is meant for ease of use, rather than advanced usage.

**rmdir** (*RmDirCommand.cs*) - This command accepts one argument. This argument should be a directory relative to the current directory or a rooted path. This directory will be removed. If `--all` is specified, all files within the directory will be removed.
**Example:** `rmdir C:/Windows --all`

 **touch** (*TouchCommand.cs*) - This command accepts one argument. This argument should be a file name. If the file exists, the last read date is set to the current time. Otherwise, the file is created.

## Etc
**clear** (*ClearCommand.cs*) - Clears the console.
**$** (*EnvCommand.cs*) - This command expects at least one argument. Fetches an environment variable with specified name. If `--format` is specified, `;` separated lists will be formatted.
**queue** (*QueueCommand.cs*) - This command will queue a command and suspend it for further use.
### Queue Arguments
`-n`, `--name` - The name of the command to queue.
`b`, `--begin` - This flag specifies whether the specified name wants to be queued. If this is set, the command will be begun and suspends.
`-d`, `--dequeue` - This flag specified whether the specified name wants to be dequeued. If this is set, the specified command is finalized and called.
`-a`, `--arguments` - The arguments to pass to the command. This is an array of arguments, so it's infinite. The command being queued will handle them so they must be correct.

**vwf** (*ViewFileCommand.cs*) - This will display the contents of a file. If the file contains a programming extension it will be automatically lexed and displayed with syntax highlighting.
<img src="https://github.com/deetonn/Console/blob/master/Console/Images/ViewFileCommand_Example_ss.png"  
alt="Image of the syntax highlighting"  
style="float: left; margin-right: 10px;" />

