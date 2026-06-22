using System.Globalization;
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
            return Brushes.Gray;

        try
        {
            var decimalValue = System.Convert.ToDecimal(value, culture);

            if (decimalValue > 0)
                return Brushes.SeaGreen;

            if (decimalValue < 0)
                return Brushes.IndianRed;

            return Brushes.Gray;
        }
        catch
        {
            return Brushes.Gray;
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
}