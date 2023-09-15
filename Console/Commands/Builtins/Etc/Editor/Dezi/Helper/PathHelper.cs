using System;
using System.IO;

namespace dezi.Helper
{
    public static class PathHelper
    {
        public readonly static string DeziConfigFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "dezi");

        public readonly static string SettingsFilePath = Path.Combine(DeziConfigFolderPath, "settings.json");

        public readonly static string KeybindingsFilePath = Path.Combine(DeziConfigFolderPath, "keybindings.json");

        public readonly static string ColorThemesFolderPath = Path.Combine(DeziConfigFolderPath, "colorThemes");

        internal static void CreateSettingsFolder()
        {
            Directory.CreateDirectory(DeziConfigFolderPath);
        }
    }
}
