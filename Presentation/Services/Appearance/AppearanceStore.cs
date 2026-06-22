using System.IO;
using System.Text.Json;

namespace DukkanDepo.Presentation.Services.Appearance;

internal static class AppearanceStore
{
    public static AppearanceSettings LoadOrDefault()
    {
        try
        {
            var path = AppearancePaths.SettingsPath;

            if (!File.Exists(path))
                return new AppearanceSettings();

            var json = File.ReadAllText(path);

            return JsonSerializer.Deserialize<AppearanceSettings>(json)
                   ?? new AppearanceSettings();
        }
        catch
        {
            return new AppearanceSettings();
        }
    }

    public static void Save(AppearanceSettings settings)
    {
        var path = AppearancePaths.SettingsPath;
        var directory = Path.GetDirectoryName(path);

        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        var json = JsonSerializer.Serialize(
            settings,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        File.WriteAllText(path, json);
    }
}