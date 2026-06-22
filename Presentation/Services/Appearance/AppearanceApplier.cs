using System.Windows.Media;
using ModernWpf;

using ThemeMgr = ModernWpf.ThemeManager;

namespace DukkanDepo.Presentation.Services.Appearance;

internal static class AppearanceApplier
{
    public static void Apply(AppearanceSettings? settings)
    {
        settings ??= new AppearanceSettings();

        ApplyTheme(settings);
        ApplyAccent(settings);
        ApplyFont(settings);
    }

    private static void ApplyTheme(AppearanceSettings settings)
    {
        ThemeMgr.Current.ApplicationTheme = settings.Theme switch
        {
            "Light" => ApplicationTheme.Light,
            "Dark" => ApplicationTheme.Dark,
            _ => null
        };
    }

    private static void ApplyAccent(AppearanceSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.AccentHex))
        {
            ThemeMgr.Current.AccentColor = null;
            return;
        }

        try
        {
            var color = (Color)ColorConverter.ConvertFromString(settings.AccentHex)!;
            ThemeMgr.Current.AccentColor = color;
        }
        catch
        {
            ThemeMgr.Current.AccentColor = null;
        }
    }

    private static void ApplyFont(AppearanceSettings settings)
    {
        System.Windows.Application.Current.Resources["AppFontSize"] = settings.FontSize;
    }
}