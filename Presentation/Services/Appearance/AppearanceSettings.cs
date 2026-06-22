namespace DukkanDepo.Presentation.Services.Appearance;

public sealed class AppearanceSettings
{
    /// <summary>
    /// System | Light | Dark
    /// </summary>
    public string Theme { get; set; } = "System";

    /// <summary>
    /// Örn: "#0078D4"
    /// Boş/null olursa sistem accent kullanılır.
    /// </summary>
    public string? AccentHex { get; set; }

    public double FontSize { get; set; } = 13.0;
}