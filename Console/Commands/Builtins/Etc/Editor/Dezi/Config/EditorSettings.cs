using dezi.Helper;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace dezi.Config
{
    public class EditorSettings : JsonSerializer
    {
        public ColorTheme CurrentColorTheme { get; set; }

        public KeyBindings CurrentKeyBindings { get; set; }

        public bool WrapLines { get; set; }

        public bool ShowLineNumbers { get; set; }

        private EditorSettings()
        {
        }

        public void LoadColorTheme()
        {
            // TODO
            ColorTheme.Load("default");
        }

        public void LoadKeyBindings()
        {
            // TODO
        }

        public static EditorSettings Load()
        {
            EditorSettings editorSettings;
            if (File.Exists(PathHelper.SettingsFilePath))
            {
                string serializedEditorSettings = File.ReadAllText(PathHelper.SettingsFilePath);
                editorSettings = JsonConvert.DeserializeObject<EditorSettings>(serializedEditorSettings);
            }
            else
            {
                editorSettings = Default();
            }

            editorSettings.LoadColorTheme();
            editorSettings.LoadKeyBindings();

            return editorSettings;
        }

        private static EditorSettings Default()
        {
            EditorSettings editorSettings = new EditorSettings();
            editorSettings.CurrentColorTheme = ColorTheme.Load("default");
            editorSettings.CurrentKeyBindings = new KeyBindings();
            editorSettings.WrapLines = true;
            editorSettings.ShowLineNumbers = true;
            if (!File.Exists(PathHelper.SettingsFilePath))
            {
                string jsonData = JsonConvert.SerializeObject(editorSettings);
                PathHelper.CreateSettingsFolder();
                File.WriteAllText(PathHelper.SettingsFilePath, jsonData);
            }
            return editorSettings;
        }
    }
}
