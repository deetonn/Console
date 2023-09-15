using System;
using System.Collections.Generic;
using System.Linq;

namespace dezi.Config
{
    public sealed class ColorTheme
    {
        private static readonly string DEFAULT_COLOR_THEME = "nice";

        public string Name { get; }

        public ConsoleColor ForegroundColor { get; }

        public ConsoleColor BackgroundColor { get; }

        public ConsoleColor CursorColor { get; }

        public ColorTheme(string name, ConsoleColor foregroundColor, ConsoleColor backgroundColor, ConsoleColor cursorColor)
        {
            this.Name = name;
            this.ForegroundColor = foregroundColor;
            this.BackgroundColor = backgroundColor;
            this.CursorColor = cursorColor;
        }

        public static ColorTheme Load(string colorThemeName)
        {
            IList<ColorTheme> colorThemes = GetPreexistingColorThemes();
            ColorTheme colorTheme = colorThemes.SingleOrDefault(ct => ct.Name == colorThemeName);
            if (colorTheme == null)
            {
                colorTheme = colorThemes.First(ct => ct.Name == DEFAULT_COLOR_THEME);
            }
            return colorTheme;
        }

        public static IList<ColorTheme> GetPreexistingColorThemes()
        {
            IList<ColorTheme> colorThemes = new List<ColorTheme>();

            ColorTheme defaultColorTheme = new ColorTheme(DEFAULT_COLOR_THEME, ConsoleColor.Black, ConsoleColor.White, ConsoleColor.Blue);

            colorThemes.Add(defaultColorTheme);

            return colorThemes;
        }
    }
}
