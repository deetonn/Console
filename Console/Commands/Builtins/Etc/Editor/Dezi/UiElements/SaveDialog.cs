using dezi.Input;
using dezi.UiElements.Editors;
using dezi.UiElements.StackPanel;

namespace dezi.UiElements
{
    public class SaveDialog : HorizontalStackPanel
    {
        public Editor EditorToSaveFilePath { get; }

        public SaveDialog(KeyboardInputs keyboardInputs, Editor currentEditor)
        {
            EditorToSaveFilePath = currentEditor;
            IsInteractiveElement = true;
            string inputLabel = "File path: ";
            var text = new Text(inputLabel, 1, inputLabel.Length)
            {
                MaxHeight = 1
            };
            AddLeft(text);
            AddRight(new SingleLineTextBox(keyboardInputs));
        }

        public SaveDialog(int yCoordinate, int xCoordinate, KeyboardInputs keyboardInputs, Editor currentEditor)
        {
            EditorToSaveFilePath = currentEditor;
            IsInteractiveElement = true;
            string inputLabel = "File path: ";
            var text = new Text(inputLabel, yCoordinate, xCoordinate, 1, inputLabel.Length)
            {
                MaxHeight = 1
            };
            AddLeft(text);
            AddRight(new SingleLineTextBox(keyboardInputs));
        }
    }
}
