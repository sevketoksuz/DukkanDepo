using System.Globalization;
using System.Windows.Data;

namespace DukkanDepo.Presentation.Converters;

/// <summary>
/// DataGrid SelectedItem bağlamasında NewItemPlaceholder durumunu güvenle null'a çevirir.
/// </summary>
public sealed class SelectedItemSafeConverter : IValueConverter
{
    public object? Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        return value;
    }

    public object? ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        if (value is not null && value.GetType().Name == "NamedObject")
            return null;

        return value;
    }
}