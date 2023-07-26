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
    IDictionary<ConsoleKey, OnInputCallback> Callbacks { get; }

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
    public void RegisterCallback(ConsoleKey on, OnInputCallback callback);
}

public class InputHandler : IInputHandler
{
    public IDictionary<ConsoleKey, OnInputCallback> Callbacks { get; } = new Dictionary<ConsoleKey,  OnInputCallback>();

    public StringBuilder Result { get; private set; } = new();

    public InputCallbackCode LastCode { get; private set; } = InputCallbackCode.Continue;

    public InputHandler()
    {
        RegisterCallback(ConsoleKey.Backspace, (_, e) =>
        {
            return "\b\b";
        });
    }

    public string ReadInputThenClear(Terminal parent)
    {
        ConsoleKeyInfo keyInfo;

        while (LastCode == InputCallbackCode.Continue)
        {
            keyInfo = parent.Ui.GetKey();

            if (!Callbacks.ContainsKey(keyInfo.Key))
            {
                Result.Append(keyInfo.KeyChar);
            }
            else
            {
                var callback = Callbacks[keyInfo.Key];
                Result.Append(callback(this, new InputEventArgs(keyInfo)));
            }

            //if (keyInfo.Key == ConsoleKey.UpArrow)
            //{
            //    var last = parent.CommandHistory.HandleMoveUp();
            //    if (last == null)
            //        continue;
            //    parent.Ui.Erase(Result.Length);
            //    Result = new StringBuilder(last);
            //    parent.Ui.DisplayPure(last);
            //    continue;
            //}

            //if (keyInfo.Key == ConsoleKey.DownArrow)
            //{
            //    var last = parent.CommandHistory.HandleMoveDown();
            //    if (last == null)
            //        continue;
            //    parent.Ui.Erase(Result.Length);
            //    Result = new StringBuilder(last);
            //    parent.Ui.DisplayPure(last);
            //    continue;
            //}

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
        parent.CommandHistory.AddRecentCommand(parent, result);
        return result;
    }

    public void RegisterCallback(ConsoleKey on, OnInputCallback callback)
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
