using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DukkanDepo.Presentation.Converters;

/// <summary>
/// DataGrid SelectedItem bağlamasında null, unset ve NewItemPlaceholder durumlarını güvenle yok sayar.
/// </summary>
public sealed class SelectedItemSafeConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        if (IsUnsafeDataGridValue(value))
            return Binding.DoNothing;

        return value;
    }

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        if (IsUnsafeDataGridValue(value))
            return Binding.DoNothing;

        return value;
    }

    private static bool IsUnsafeDataGridValue(object? value)
    {
        if (value is null)
            return true;

        if (value == DependencyProperty.UnsetValue)
            return true;

        if (value == Binding.DoNothing)
            return true;

        var typeName = value.GetType().Name;

        return typeName is "NamedObject" or "NewItemPlaceholder";
    }
}