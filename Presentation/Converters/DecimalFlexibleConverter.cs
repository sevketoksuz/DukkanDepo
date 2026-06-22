using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DukkanDepo.Presentation.Converters;

public sealed class DecimalFlexibleConverter : IValueConverter
{
    public object? Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        return value switch
        {
            decimal decimalValue => decimalValue.ToString("N2", culture),
            null => null,
            _ => value
        };
    }

    public object? ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        var text = value?.ToString()?.Trim();

        if (string.IsNullOrWhiteSpace(text))
            return null;

        text = text
            .Replace("₺", string.Empty)
            .Replace("$", string.Empty)
            .Replace("€", string.Empty)
            .Replace(" ", string.Empty);

        if (decimal.TryParse(text, NumberStyles.Any, culture, out var decimalValue) ||
            decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimalValue))
        {
            return decimalValue;
        }

        return DependencyProperty.UnsetValue;
    }
}