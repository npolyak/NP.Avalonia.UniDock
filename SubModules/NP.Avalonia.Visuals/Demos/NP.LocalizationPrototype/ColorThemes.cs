namespace NP.LocalizationPrototype
{
    public enum ColorTheme
    {
        Dark,
        Light
    }

    public static class ColorThemesHelper
    {
        public static ColorTheme[] ColorThemes { get; } = 
            {
                ColorTheme.Dark, 
                ColorTheme.Light };
    }
}
