using System.Globalization;
using System.Windows.Data;

namespace DukkanDepo.Presentation.Converters;

public sealed class FontSizeToMaxHeightConverter : IValueConverter
{
    public double Factor { get; set; } = 1.35;

    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        if (value is not double fontSize)
            return value;

        var lines = 2;

        if (parameter is not null &&
            int.TryParse(parameter.ToString(), out var parsedLines))
        {
            lines = parsedLines;
        }

        return fontSize * Factor * lines;
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