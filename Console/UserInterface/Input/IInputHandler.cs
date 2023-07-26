using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console.UserInterface.Input;

using NativeConsole = global::System.Console;

public record class InputEventArgs(ConsoleKeyInfo KeyInfo);
public delegate string OnInputCallback(IInputHandler handler, InputEventArgs e);

public enum InputCallbackCode
{
    Stop,
    Continue
}

public interface IInputHandler
{
    /// <summary>
    /// These callbacks shall be called when the user presses a key.
    /// If the callback returns null, the input will be ignored.
    /// If the callback returns a string, the input will be replaced with the string.
    /// If the dictionary does not contain a key for that input, it is just added.
    /// </summary>
    IDictionary<char, OnInputCallback> Callbacks { get; }

    /// <summary>
    /// This is the final product. The built string.
    /// </summary>
    StringBuilder Result { get; }

    /// <summary>
    /// This is the code that tells the input handler if it should stop
    /// now. I.E if ENTER has been entered.
    /// </summary>
    InputCallbackCode LastCode { get; }

    /// <summary>
    /// Set <see cref="LastCode"/> and notify the input handler that it should stop.
    /// </summary>
    /// <param name="code">The code</param>
    public void SetCallbackCode(InputCallbackCode code);

    /// <summary>
    /// Read input from the user, executing any callbacks.
    /// Stopping when <see cref="LastCode"/> becomes <see cref="InputCallbackCode.Stop"/>
    /// </summary>
    /// <returns>The input</returns>
    public string ReadInputThenClear(Terminal parent);

    /// <summary>
    /// Add a callback for when <paramref name="on"/> is pressed.
    /// </summary>
    /// <param name="on">The character to execute the callback on.</param>
    /// <param name="callback">The callback to execute.</param>
    public void RegisterCallback(char on, OnInputCallback callback);
}

public class InputHandler : IInputHandler
{
    public IDictionary<char, OnInputCallback> Callbacks { get; } = new Dictionary<char,  OnInputCallback>();

    public StringBuilder Result { get; } = new();

    public InputCallbackCode LastCode { get; private set; } = InputCallbackCode.Continue;

    public string ReadInputThenClear(Terminal parent)
    {
        ConsoleKeyInfo keyInfo;

        while (LastCode == InputCallbackCode.Continue)
        {
            keyInfo = NativeConsole.ReadKey(true);

            if (!Callbacks.ContainsKey(keyInfo.KeyChar))
            {
                Result.Append(keyInfo.KeyChar);
            }
            else
            {
                var callback = Callbacks[keyInfo.KeyChar];
                Result.Append(callback(this, new InputEventArgs(keyInfo)));
            }

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                // Enter is an immediate return
                parent.Ui.DisplayPure("\n");
                return Result.ToString();
            }
            else
            {
                parent.Ui.DisplayPure($"{keyInfo.KeyChar}");
            }
        }

        var result = Result.ToString();
        return result;
    }

    public void RegisterCallback(char on, OnInputCallback callback)
    {
        Callbacks[on] = callback;
    }

    public void SetCallbackCode(InputCallbackCode code)
    {
        LastCode = code;
    }

    public void ResetState()
    {
        Result.Clear();
        LastCode = InputCallbackCode.Continue;
    }
}
