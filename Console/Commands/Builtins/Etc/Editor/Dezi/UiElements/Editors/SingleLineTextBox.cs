using dezi.Helper;
using dezi.Input;
using System;
using System.Collections.Generic;

namespace dezi.UiElements.Editors
{
    public class SingleLineTextBox : UiElement
    {
        private readonly KeyboardInputs keyboardInputs;

        public Cursor Cursor { get; set; }

        public string InputText { get; set; }

        public SingleLineTextBox(KeyboardInputs keyboardInputs, string inputText = "")
        {
            this.InputText = inputText;
            this.MaxHeight = 1;
            this.Cursor = new Cursor(0, 0);
            this.IsInteractiveElement = true;
        }

        public override void HandleInput(InputAction inputAction)
        {
            // TODO
            switch (inputAction)
            {
                case InputAction.MoveCursorLeft:
                    this.Cursor.UpdateColumn(-1, this.GetInputTextAsList());
                    break;
                case InputAction.MoveCursorRight:
                    this.Cursor.UpdateColumn(+1, this.GetInputTextAsList());
                    break;
                case InputAction.MoveCursorDown:
                case InputAction.PageDown:
                case InputAction.End:
                    this.Cursor.UpdateColumnAbsolute(-1, this.GetInputTextAsList());
                    break;
                case InputAction.MoveCursorUp:
                case InputAction.PageUp:
                case InputAction.Home:
                    this.Cursor.UpdateColumnAbsolute(0, this.GetInputTextAsList());
                    break;
                case InputAction.Cancel:
                case InputAction.QuitUiElement:
                    break;
                case InputAction.Delete:
                    if (this.InputText.Length > this.Cursor.Column)
                    {
                        this.InputText.Remove(this.Cursor.Column, 1);
                    }
                    break;
                case InputAction.Backspace:
                    if (this.Cursor.Column > 0)
                    {
                        this.InputText.Remove(this.Cursor.Column - 1, 1);
                    }
                    break;
                case InputAction.AddNewLine:
                    break;
                case InputAction.Input:
                    this.InputText = StringHelper.PutInNewStringAtIndex(this.InputText, this.Cursor.Column, keyboardInputs.LatestInput);
                    break;
                case InputAction.Copy:
                    break;
                case InputAction.Paste:
                    break;
                default:
                    break;
            }
        }

        public override void Render(IList<string> uiOutput)
        {
            string inputToDisplay = this.InputText;
            if (inputToDisplay.Length > this.Width)
            {
                inputToDisplay = inputToDisplay.Substring(Math.Max(0, inputToDisplay.Length - Cursor.Column), this.Width);
            }
            uiOutput[this.CoordinateY] = StringHelper.PutInNewStringAtIndex(uiOutput[this.CoordinateY], this.CoordinateX, inputToDisplay);
        }

        protected override void UpdateSubmoduleSizesAndPositions(int widthDelta, int heightDelta)
        {
            // no submodules so nothing to do
        }

        private IList<string> GetInputTextAsList()
        {
            return new List<string>() { this.InputText };
        }

        public override IList<Cursor> GetCursors()
        {
            return new List<Cursor>() { this.Cursor };
        }
    }
}
