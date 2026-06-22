using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace DukkanDepo.Presentation.Helpers;

public static class DataGridTemplateEditHelper
{
    public static bool IsNumericEditColumn(string sortMemberPath)
    {
        return sortMemberPath is "Adet" or "Satis" or "Iskonto";
    }

    public static string? GetNumericTextFromKey(Key key)
    {
        if (key >= Key.D0 && key <= Key.D9)
            return ((int)key - (int)Key.D0).ToString();

        if (key >= Key.NumPad0 && key <= Key.NumPad9)
            return ((int)key - (int)Key.NumPad0).ToString();

        return key switch
        {
            Key.Decimal => ",",
            Key.OemComma => ",",
            Key.OemPeriod => ".",
            _ => null
        };
    }

    public static bool IsAllowedFirstInput(
        string sortMemberPath,
        string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        if (sortMemberPath == "Adet")
            return text.All(char.IsDigit);

        if (sortMemberPath is "Satis" or "Iskonto")
            return text.All(char.IsDigit);

        return false;
    }

    public static async Task BeginEditAsync(
        DataGrid grid,
        object item,
        DataGridColumn column,
        string? initialText)
    {
        grid.Focus();
        grid.SelectedItem = item;
        grid.CurrentCell = new DataGridCellInfo(item, column);
        grid.ScrollIntoView(item, column);
        grid.UpdateLayout();

        var cell = GetCell(grid, item, column);

        if (cell is not null)
        {
            cell.Focus();
            cell.IsSelected = true;
        }

        grid.BeginEdit();

        await grid.Dispatcher.InvokeAsync(
            () => { },
            DispatcherPriority.ContextIdle);

        var textBox = FindTextBoxInCurrentCell(grid, item, column);

        if (textBox is null)
            return;

        textBox.Focus();

        if (initialText is not null)
        {
            textBox.Text = initialText;
            textBox.CaretIndex = textBox.Text.Length;
            textBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
        }
        else
        {
            textBox.SelectAll();
        }
    }

    private static TextBox? FindTextBoxInCurrentCell(
        DataGrid grid,
        object item,
        DataGridColumn column)
    {
        var cell = GetCell(grid, item, column);

        if (cell is null)
            return null;

        return FindVisualChild<TextBox>(cell);
    }

    private static DataGridCell? GetCell(
        DataGrid grid,
        object item,
        DataGridColumn column)
    {
        var row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;

        if (row is null)
        {
            grid.ScrollIntoView(item, column);
            grid.UpdateLayout();

            row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
        }

        if (row is null)
            return null;

        var presenter = FindVisualChild<DataGridCellsPresenter>(row);

        if (presenter is null)
        {
            grid.ScrollIntoView(item, column);
            grid.UpdateLayout();

            presenter = FindVisualChild<DataGridCellsPresenter>(row);
        }

        if (presenter is null)
            return null;

        return presenter
            .ItemContainerGenerator
            .ContainerFromIndex(column.DisplayIndex) as DataGridCell;
    }

    private static T? FindVisualChild<T>(DependencyObject? parent)
        where T : DependencyObject
    {
        if (parent is null)
            return null;

        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);

            if (child is T typedChild)
                return typedChild;

            var result = FindVisualChild<T>(child);

            if (result is not null)
                return result;
        }

        return null;
    }
}