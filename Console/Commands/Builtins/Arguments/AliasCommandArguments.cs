

using CommandLine;

namespace Console.Commands.Builtins.Arguments;

public class AliasCommandArguments
{

    [Option('n', "name", HelpText = "The name of the alias.", Required = true)]
    public string? AliasName { get; set; }

    [Option('d', "desc", HelpText = "The description of the newly created command.")]
    public string? AliasDesc { get; set; } = "This is an alias command.";

    [Option('s', "script", HelpText = "The script to execute when --name is ran.", Required = true)]
    public string? Script { get; set; }
}
