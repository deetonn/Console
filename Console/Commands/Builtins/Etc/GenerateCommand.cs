using Console.Errors;
using System.Security.Cryptography;

namespace Console.Commands.Builtins.Etc;

public class GenerateCommand : BaseBuiltinCommand
{
    public override string Name => "generate";
    public override string Description => "generate different things. use --help for more information.";

    public override CommandResult Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        if (args.Count == 0)
        {
            return DoHelp();
        }

        var mode = args[0];

        return mode switch
        {
            "password" => PasswordPath([.. args.ToArray()[1..]]),
            "integer" => RandomIntegerPath(args.Contains("--long")),
            _ => DoHelp(),
        };
    }

    public int RandomIntegerPath(bool wantsLong)
    {
        if (wantsLong)
        {
            var random = Random.Shared.NextInt64();
            WriteLine($"{random}");
        }
        else
        {
            var random = Random.Shared.Next();
            WriteLine($"{random}");
        }

        return 0;
    }

    public CommandResult PasswordPath(List<string> args)
    {
        if (args.Count == 0)
        {
            return Error()
                .WithMessage("the argument \"length: number\" was not specified.")
                .Build();
        }

        if (!int.TryParse(args[0], out var length))
        {
            return Error()
                .WithMessage("length argument is invalid")
                .WithNote("the number is unable to be parsed")
                .Build();
        }

        var password = GeneratePassword(length);
        WriteLine($"{password}");

        return 0;
    }

    private static string GeneratePassword(int size)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@!";
        return new string(Enumerable.Repeat(chars, size)
                       .Select(s => s[RandomNumberGenerator.GetInt32(0, s.Length)]).ToArray());
    }

    CommandError DoHelp()
    {
        return Error()
            .WithMessage("invalid usage")
            .WithNote($"use \"docs {Name}\" for more information.")
            .Build();
    }

    public override string DocString => $@"
This command can generate random strings & numbers based on user input.

The options are:
  password: generate a password
    [[count: number]] - The number of characters for the password to contain.
  integer: generate a random number
    [[--long: flag]] if present, the number generated will be 64-bit, instead of 32-bit.

Example usages:
  generate password 16
    ^ will generate a string of length 16, containing entirely random characters.
  generate integer --long
    ^ will generate a 64-bit random integer.

All results are outputted to the console.
";
}
