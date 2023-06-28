using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console.Commands.Builtins.Etc;

public class EnvCommand : BaseBuiltinCommand
{
    public override string Name => "$";

    public override string Description => "Fetch an environment variable.";

    public override int Run(List<string> args, Terminal parent)
    {
        base.Run(args, parent);

        if (args.Count == 0 || args.Contains("--help"))
        {
            WriteLine($"{Name} - usage");
            WriteLine($"  {Name} <name> [...options]");
            WriteLine($"    --format: format the output.");
            return 0;
        }

        var format = args.Contains("--format");
        var variable = args[0];

        if (string.IsNullOrWhiteSpace(variable))
        {
            parent.WriteLine("env: missing variable name");
            parent.WriteLine($"Try '{Name} --help' for more information.");
            return CommandReturnValues.BadArguments;
        }

        var value = Environment.GetEnvironmentVariable(variable);

        if (value is null)
        {
            WriteLine($"{Name}: No such environment variable `{variable}`");
            return -1;
        }

        if (format)
        {
            // check if the variable is a `;` seperated list.
            if (value.Contains(';'))
            {
                // format it as such.
                foreach (var thing in value.Split(';'))
                {
                    WriteLine($"{thing}");
                }
            }
            else
            {
                WriteLine($"{variable}={value}");
            }
        }
        else
        {
            WriteLine(value);
        }

        return 0;
    }
}
