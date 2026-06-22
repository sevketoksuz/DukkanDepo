using System.Globalization;
using System.Windows.Data;

namespace DukkanDepo.Presentation.Converters;

public sealed class FontSizeToLineHeightConverter : IValueConverter
{
    public double Factor { get; set; } = 1.35;

    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        return value is double fontSize
            ? fontSize * Factor
            : value;
    }

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}