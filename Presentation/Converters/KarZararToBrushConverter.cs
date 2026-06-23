using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace DukkanDepo.Presentation.Converters;

public sealed class KarZararToBrushConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        if (value is null)
            return GetBrush("TextFillColorSecondaryBrush", Brushes.Gray);

        try
        {
            var decimalValue = System.Convert.ToDecimal(value, culture);

            if (decimalValue > 0)
                return GetBrush("SystemFillColorSuccessBrush", Brushes.SeaGreen);

            if (decimalValue < 0)
                return GetBrush("SystemFillColorCriticalBrush", Brushes.IndianRed);

            return GetBrush("TextFillColorSecondaryBrush", Brushes.Gray);
        }
        catch
        {
            return GetBrush("TextFillColorSecondaryBrush", Brushes.Gray);
        }
    }

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    private static Brush GetBrush(string resourceKey, Brush fallback)
    {
        try
        {
            if (System.Windows.Application.Current?.TryFindResource(resourceKey) is Brush brush)
                return brush;
        }
        catch
        {
            // Theme resource geçici olarak çözülemezse uygulamayı düşürme.
        }

        return fallback;
    }
}