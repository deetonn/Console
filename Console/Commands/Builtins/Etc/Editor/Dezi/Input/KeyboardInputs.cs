using dezi.Config;
using System;

using SystemConsole = global::System.Console;

namespace dezi.Input
{
    public class KeyboardInputs
    {
        public string LatestInput { get; private set; }

        public KeyBindings KeyBindings { get; set; }

        public KeyboardInputs(KeyBindings keyBindings)
        {
            this.KeyBindings = keyBindings;
        }

        public InputAction GetInputActionsFromKeyboard(DeziStatus deziStatus)
        {
            // TODO: refactor with new class that maps shortcuts to action
            // TODO: alt key doesn't work
            ConsoleKeyInfo character = SystemConsole.ReadKey(true);
            // TODO: use key bindings from settings and not fix keys
            if (IsPressingCtrl(character) && character.Key == ConsoleKey.Q)
            {
                return InputAction.QuitProgram;
            }
            else if (IsPressingCtrl(character) && character.Key == ConsoleKey.W)
            {
                return InputAction.QuitUiElement;
            }

            if (deziStatus == DeziStatus.EditingFile)
            {
                if (IsPressingCtrl(character) && character.Key == ConsoleKey.S)
                {
                    return InputAction.Save;
                }
                else if (IsPressingCtrl(character) && character.Key == ConsoleKey.U)
                {
                    return InputAction.SpawnMultiCursorAbove;
                }
                else if (IsPressingCtrl(character) && character.Key == ConsoleKey.I)
                {
                    return InputAction.SpawnMultiCursorUnder;
                }
                else if (character.Key == ConsoleKey.Escape)
                {
                    return InputAction.DeactivateMultiCursors;
                }
                else if (character.Key == ConsoleKey.UpArrow)
                {
                    return InputAction.MoveCursorUp;
                }
                else if (character.Key == ConsoleKey.DownArrow)
                {
                    return InputAction.MoveCursorDown;
                }
                else if (character.Key == ConsoleKey.LeftArrow)
                {
                    return InputAction.MoveCursorLeft;
                }
                else if (character.Key == ConsoleKey.RightArrow)
                {
                    return InputAction.MoveCursorRight;
                }
                else if (character.Key == ConsoleKey.Backspace)
                {
                    return InputAction.Backspace;
                }
                else if (character.Key == ConsoleKey.Delete)
                {
                    return InputAction.Delete;
                }
                else if (character.Key == ConsoleKey.Enter)
                {
                    return InputAction.AddNewLine;
                }
                else if (character.Key == ConsoleKey.Home)
                {
                    return InputAction.Home;
                }
                else if (character.Key == ConsoleKey.End)
                {
                    return InputAction.End;
                }
                else if (character.Key == ConsoleKey.PageUp)
                {
                    return InputAction.PageUp;
                }
                else if (character.Key == ConsoleKey.PageDown)
                {
                    return InputAction.PageDown;
                }
                else if (character.Key == ConsoleKey.Tab)
                {
                    return InputAction.Tab;
                }
            }
            else if (deziStatus == DeziStatus.SaveFile)
            {
                if (character.Key == ConsoleKey.Escape)
                {
                    return InputAction.Cancel;
                }
                else if (character.Key == ConsoleKey.Enter)
                {
                    return InputAction.Save;
                }
            }

            this.LatestInput = character.KeyChar.ToString();
            return InputAction.Input;
        }

        private bool IsPressingCtrl(ConsoleKeyInfo character)
        {
            return (ConsoleModifiers.Control & character.Modifiers) != 0;
        }

        private bool IsPressingShift(ConsoleKeyInfo character)
        {
            return (ConsoleModifiers.Shift & character.Modifiers) != 0;
        }

        private bool IsPressingAlt(ConsoleKeyInfo character)
        {
            return (ConsoleModifiers.Alt & character.Modifiers) != 0;
        }
    }
}
