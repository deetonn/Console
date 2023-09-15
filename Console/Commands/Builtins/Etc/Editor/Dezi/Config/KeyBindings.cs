using dezi.Helper;
using System.IO;

namespace dezi.Config
{
    public class KeyBindings
    {
        // TODO: load settings
        // TODO: create properties to load settings into

        public KeyBindings()
        {
        }

        public static KeyBindings DefaultKeyBindings()
        {
            KeyBindings keyBindings = new KeyBindings();
            // TODO
            return keyBindings;
        }

        public static KeyBindings Load()
        {
            if (File.Exists(PathHelper.KeybindingsFilePath))
            {
                // TODO: load file
            }
            return DefaultKeyBindings();
        }
    }
}
