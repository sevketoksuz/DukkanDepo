namespace DukkanDepo.Presentation.Services.Appearance;

internal static class AppearancePaths
{
    public static string SettingsPath =>
        System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "DukkanDepo",
            "appearance.json");
}