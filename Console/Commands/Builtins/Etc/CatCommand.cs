using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console.Errors;
using Console.Extensions;

namespace Console.Commands.Builtins.Etc;

public class CatCommand : BaseBuiltinCommand
{
    public override string Name => "cat";
    public override string Description => "View/Edit the contents of a file.";

    public override CommandResult Run(List<string> args, IConsole parent)
    {
        base.Run(args, parent);

        if (args.Count < 1) 
        {
            return Error()
                .WithMessage("Expected at least one argument. (the filename)")
                .WithNote($"example: `{Name} /proc/sysinfo`")
                .WithNote($"the first argument must be the file, proceeded by flags.")
                .Build();
        }

        var filePath = args[0];
        if (!File.Exists(filePath))
        {
            return Error()
                .WithMessage($"The file `{filePath}` could not be found.")
                .WithNote("The flag `-w` will soon be your route to creating the file.")
                .Build();
        }

        var contents = File.ReadAllText(filePath);
        var buffer = contents.Split(Environment.NewLine);
        var position = 0;

        if (buffer.Length < parent.Ui.BufferHeight)
        {
            WriteLine($"FILE: {filePath}, LENGTH: {contents.Length}b");
            DisplayBuffer(buffer, position, parent);
            return 0;
        }

        ConsoleKeyInfo keyInfo;
        while ((keyInfo = parent.Ui.GetKey()).Key != ConsoleKey.Q)
        {
            DisplayBuffer(buffer, position, parent);
            Write($"(q for exit): ");

            if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                position++;
                continue;
            }
        }

        return 0;
    }

    void DisplayBuffer(string[] lines, int position, IConsole parent)
    {
        // We show however many lines include the line at position 
        // after position that the console buffer can fit, minus one.
        // The -1 is to display the prompt.
        parent.Ui.Clear();
        var height = parent.Ui.BufferHeight;

        if (position > lines.Length)
        {
            WriteLine("**** END ****");
            return;
        }

        if (lines.Length < height)
        {
            // We are just outputting the lines and not doing any functionality.
            foreach (var line in lines)
                WriteLine($"{line.MarkupStrip()}");
            return;
        }

        for (var i = 0; i < height - 1; ++i)
        {
            var line = lines[i];
            WriteLine($"{line}");
        }
    }
}
