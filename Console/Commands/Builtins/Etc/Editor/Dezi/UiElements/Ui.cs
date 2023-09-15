using dezi.Config;
using dezi.Input;
using dezi.UiElements.Editors;
using dezi.UiElements.StackPanel;
using System;
using System.Collections.Generic;
using System.Linq;

using SystemConsole = global::System.Console;

namespace dezi.UiElements
{
    /// <summary>
    /// Creates the UI or the sub-elements in it
    /// </summary>
    public class Ui
    {
        private bool isFirstRender;

        public static int TerminalWidth => SystemConsole.WindowWidth;

        public static int TerminalHeight => SystemConsole.WindowHeight;

        public DeziStatus DeziStatus { get; set; }

        private IList<string> uiOutput;

        public IList<string> UiOutput
        {
            get
            {
                // change array size to terminal size
                if (this.uiOutput.Count > TerminalHeight)
                {
                    this.uiOutput = this.uiOutput.Take(TerminalHeight).ToArray();
                }
                else if (this.uiOutput.Count < TerminalHeight)
                {
                    do
                    {
                        string emptyLine = string.Empty.PadRight(TerminalWidth);
                        this.uiOutput.Add(emptyLine);
                    }
                    while (this.uiOutput.Count < TerminalHeight);
                }

                // change string length to new length
                if (this.uiOutput.First().Length > TerminalWidth)
                {
                    this.uiOutput = this.uiOutput
                        .Select(l => l.Substring(0, TerminalWidth))
                        .ToArray();
                }
                else if (this.uiOutput.First().Length < TerminalWidth)
                {
                    int lengthDifference = TerminalWidth - this.uiOutput.First().Length;
                    this.uiOutput = this.uiOutput
                        .Select(l => l.PadRight(TerminalWidth - lengthDifference))
                        .ToArray();
                }
                return this.uiOutput;
            }
        }

        public IList<Editor> Editors { get; set; }

        public KeyboardInputs KeyboardInputs { get; }

        public EditorSettings EditorSettings { get; set; }

        public VerticalStackPanel BaseVerticalStackPanel { get; set; }

        public Ui(IList<string> filePaths)
        {
            isFirstRender = true;

            this.EditorSettings = EditorSettings.Load();
            this.KeyboardInputs = new KeyboardInputs(this.EditorSettings.CurrentKeyBindings);

            this.uiOutput = new List<string>();
            for (int i = 0; i < TerminalHeight; i++)
            {
                this.uiOutput.Add(string.Empty.PadRight(TerminalWidth));
            }

            if (filePaths.Count == 0)
            {
                // open empty editor when no file path was given
                filePaths = new string[1] { "" };
            }

            this.Editors = new List<Editor>
            {
                new Editor(this.KeyboardInputs, 0, 0, TerminalWidth, TerminalHeight - 1, true, this.ChangeToSaveStatus, filePaths[0])
            };

            this.BaseVerticalStackPanel = new VerticalStackPanel();
            this.BaseVerticalStackPanel.AddTop(this.Editors.First());
            HorizontalStackPanel horizontalStackPanelForCommands = new HorizontalStackPanel();
            horizontalStackPanelForCommands.MaxHeight = 1;
            this.BaseVerticalStackPanel.AddBottom(horizontalStackPanelForCommands);

            SystemConsole.Title = "Dezi";
            SystemConsole.CursorVisible = false;
            // TODO: replace with multi-platform code
            SystemConsole.SetWindowSize(TerminalWidth, TerminalHeight);
            SystemConsole.SetBufferSize(TerminalWidth, TerminalHeight);
        }

        public void Run()
        {
            while (true)
            {
                Render();
                HandleInput();
            }
        }

        private void Render()
        {
            SystemConsole.BackgroundColor = this.EditorSettings.CurrentColorTheme.BackgroundColor;
            SystemConsole.ForegroundColor = this.EditorSettings.CurrentColorTheme.ForegroundColor;

            // TODO: replace copy-method with non-deprecated method
            IList<string> oldUiOutput = uiOutput.Select(l => string.Copy(l)).ToList();
            bool consoleSizeChanged = uiOutput.Count != TerminalHeight || this.uiOutput.First().Count() != TerminalWidth;
            if (consoleSizeChanged)
            {
                uiOutput = new List<string>();
                for (int y = 0; y < TerminalHeight; y++)
                {
                    uiOutput.Add(new string(' ', TerminalWidth));
                }
            }

            BaseVerticalStackPanel.Height = TerminalHeight;
            BaseVerticalStackPanel.Width = TerminalWidth;
            BaseVerticalStackPanel.Render(uiOutput);

            // output to terminal
            // only update the lines that changed
            // updating complete line is faster than checking every char.
            for (int rowIndex = 0; rowIndex < this.UiOutput.Count; rowIndex++)
            {
                if (this.isFirstRender || consoleSizeChanged || this.UiOutput[rowIndex] != oldUiOutput[rowIndex])
                {
                    SetTerminalCursorPosition(rowIndex, 0);
                    SystemConsole.Write(this.UiOutput[rowIndex]);
                }
            }

            foreach (Cursor cursor in BaseVerticalStackPanel.GetInteractiveUiElements().Single(iue => iue.IsInFocus).GetCursors())
            {
                SetTerminalCursorPosition(cursor.Row, cursor.Column);
                SystemConsole.BackgroundColor = EditorSettings.CurrentColorTheme.CursorColor;
                SystemConsole.Write(uiOutput[cursor.Row][cursor.Column]);
                SystemConsole.ResetColor();
            }
        }

        private void SetTerminalCursorPosition(int row, int column)
        {
            SystemConsole.CursorTop = row;
            SystemConsole.CursorLeft = column;
        }

        private void HandleInput()
        {
            InputAction inputAction = this.KeyboardInputs.GetInputActionsFromKeyboard(this.DeziStatus);
            switch (inputAction)
            {
                // add universal actions in here
                case InputAction.QuitProgram:
                    QuitProgram();
                    break;
                case InputAction.QuitUiElement:
                    this.Editors.Remove(this.Editors.Single(e => e.IsInFocus));
                    if (this.Editors.Count == 0)
                    {
                        QuitProgram();
                    }
                    else
                    {
                        this.Editors.First().IsInFocus = true;
                    }
                    break;
                default:
                    if (this.DeziStatus == DeziStatus.EditingFile)
                    {
                        this.BaseVerticalStackPanel.GetInteractiveUiElements().Single(iue => iue.IsInFocus).HandleInput(inputAction);
                    }
                    else if (this.DeziStatus == DeziStatus.SaveFile)
                    {

                    }
                    break;
            }
        }

        private void QuitProgram()
        {
            SystemConsole.Clear();
            Environment.Exit(0);
        }

        public void ChangeToSaveStatus()
        {
            //////////////////////////////
            // TODO: finish this
            // must be editor, otherwise it wouldn't be in save status
            Editor currentEditor = (Editor)this.BaseVerticalStackPanel.GetInteractiveUiElements().Single(iue => iue.IsInFocus);
            currentEditor.IsInFocus = false;

            this.DeziStatus = DeziStatus.SaveFile;
            SaveDialog saveDialog = new SaveDialog(this.KeyboardInputs, currentEditor);
            saveDialog.IsInFocus = true;
            this.BaseVerticalStackPanel.AddBottom(saveDialog);
        }
    }
}
