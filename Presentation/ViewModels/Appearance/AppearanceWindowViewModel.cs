using System.Windows.Input;
using System.Windows.Media;
using DukkanDepo.Presentation.Commands;
using DukkanDepo.Presentation.Common;
using DukkanDepo.Presentation.Services.Appearance;

namespace DukkanDepo.Presentation.ViewModels.Appearance;

public sealed class AppearanceWindowViewModel : ObservableObject
{
    public event Action? RequestClose;

    public event Action<string, string>? RequestMessage;

    private string _theme = "System";

    private string? _accentHex;

    private double _fontSize = 13.0;

    public AppearanceWindowViewModel()
    {
        LoadFromCurrent();

        SaveCommand = new RelayCommand(_ => Save());
        DefaultsCommand = new RelayCommand(_ => Defaults());
        CloseCommand = new RelayCommand(_ => RequestClose?.Invoke());
        PickAccentCommand = new RelayCommand(parameter =>
        {
            if (parameter is string hex)
                AccentHex = hex;
        });
    }

    public string Theme
    {
        get => _theme;
        set => SetProperty(ref _theme, value);
    }

    public string? AccentHex
    {
        get => _accentHex;
        set
        {
            if (!SetProperty(ref _accentHex, value))
                return;

            OnPropertyChanged(nameof(AccentPreviewBrush));
        }
    }

    public double FontSize
    {
        get => _fontSize;
        set => SetProperty(ref _fontSize, value);
    }

    public Brush AccentPreviewBrush
    {
        get
        {
            if (TryParseColor(AccentHex, out var color))
                return new SolidColorBrush(color);

            return Brushes.Transparent;
        }
    }

    public ICommand SaveCommand { get; }

    public ICommand DefaultsCommand { get; }

    public ICommand CloseCommand { get; }

    public ICommand PickAccentCommand { get; }

    private void LoadFromCurrent()
    {
        var settings = AppearanceService.Current;

        Theme = string.IsNullOrWhiteSpace(settings.Theme)
            ? "System"
            : settings.Theme;

        AccentHex = settings.AccentHex;

        FontSize = settings.FontSize <= 0
            ? 13.0
            : settings.FontSize;

        OnPropertyChanged(nameof(AccentPreviewBrush));
    }

    private void Defaults()
    {
        var settings = new AppearanceSettings();

        Theme = settings.Theme;
        AccentHex = settings.AccentHex;
        FontSize = settings.FontSize;

        AppearanceService.SaveAndApply(settings);

        OnPropertyChanged(nameof(AccentPreviewBrush));
    }

    private void Save()
    {
        if (!string.IsNullOrWhiteSpace(AccentHex) &&
            !TryParseColor(AccentHex, out _))
        {
            RequestMessage?.Invoke(
                "Geçerli bir HEX renk gir. Örnek: #0078D4",
                "Uyarı");

            return;
        }

        var settings = new AppearanceSettings
        {
            Theme = string.IsNullOrWhiteSpace(Theme)
                ? "System"
                : Theme,

            AccentHex = string.IsNullOrWhiteSpace(AccentHex)
                ? null
                : AccentHex.Trim(),

            FontSize = FontSize <= 0
                ? 13.0
                : FontSize
        };

        AppearanceService.SaveAndApply(settings);

        RequestClose?.Invoke();
    }

    private static bool TryParseColor(
        string? hex,
        out Color color)
    {
        color = default;

        if (string.IsNullOrWhiteSpace(hex))
            return false;

        try
        {
            var converted = ColorConverter.ConvertFromString(hex.Trim());

            if (converted is not Color parsedColor)
                return false;

            color = parsedColor;
            return true;
        }
        catch
        {
            return false;
        }
    }
}