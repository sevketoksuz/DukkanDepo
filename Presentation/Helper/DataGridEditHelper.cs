using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace DukkanDepo.Presentation.Helpers;

public static class DataGridEditHelper
{
    public static void CommitAllEdits(DataGrid grid)
    {
        if (grid is null)
            return;

        try
        {
            if (grid.Items is IEditableCollectionView editableCollectionView)
            {
                if (editableCollectionView.IsAddingNew)
                    editableCollectionView.CancelNew();

                if (editableCollectionView.IsEditingItem)
                    editableCollectionView.CancelEdit();
            }
        }
        catch
        {
            // Best effort.
        }

        try
        {
            grid.CommitEdit(DataGridEditingUnit.Cell, true);
        }
        catch
        {
            // Best effort.
        }

        try
        {
            grid.CommitEdit(DataGridEditingUnit.Row, true);
        }
        catch
        {
            // Best effort.
        }
    }

    public static bool IsPlaceholder(object? value)
    {
        return value is not null && value.GetType().Name == "NamedObject";
    }

    public static TextBox? TryGetFocusedTextBox()
    {
        return Keyboard.FocusedElement as TextBox;
    }

    public static bool IsEditingCell(DataGrid grid)
    {
        DependencyObject? dependencyObject = Keyboard.FocusedElement as DependencyObject;

        while (dependencyObject is not null)
        {
            if (dependencyObject is DataGridCell cell)
                return cell.IsEditing;

            if (dependencyObject is DataGridRow row)
                return row.IsEditing;

            dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
        }

        return false;
    }

    public static void PrepareTextBoxForEdit(
        TextBox textBox,
        bool initiatedByTyping)
    {
        if (textBox is null)
            return;

        if (initiatedByTyping)
        {
            textBox.Dispatcher.InvokeAsync(() =>
            {
                textBox.Focus();
                textBox.CaretIndex = textBox.Text?.Length ?? 0;
            }, System.Windows.Threading.DispatcherPriority.Background);
        }
        else
        {
            textBox.Dispatcher.InvokeAsync(() =>
            {
                textBox.Focus();
                textBox.SelectAll();
            }, System.Windows.Threading.DispatcherPriority.Background);
        }
    }

    public static void SafeRestoreFocus(
        DataGrid grid,
        object item,
        DataGridColumn column)
    {
        if (grid is null || item is null || column is null)
            return;

        if (IsPlaceholder(item))
            return;

        try
        {
            var exists = grid.Items is not null &&
                         grid.Items
                             .Cast<object>()
                             .Any(existingItem => ReferenceEquals(existingItem, item));

            if (!exists)
                return;

            grid.SelectedItem = item;

            DataGridCellInfo cellInfo;

            try
            {
                cellInfo = new DataGridCellInfo(item, column);
            }
            catch (ArgumentNullException)
            {
                return;
            }

            grid.CurrentCell = cellInfo;

            grid.Dispatcher.InvokeAsync(() =>
            {
                grid.UpdateLayout();
                grid.ScrollIntoView(item, column);

                var row = (DataGridRow?)grid.ItemContainerGenerator.ContainerFromItem(item);

                if (row is null)
                    return;

                var presenter = FindVisualChild<DataGridCellsPresenter>(row);

                if (presenter is null)
                    return;

                var cell = (DataGridCell?)presenter
                    .ItemContainerGenerator
                    .ContainerFromIndex(column.DisplayIndex);

                cell?.Focus();
            }, System.Windows.Threading.DispatcherPriority.Background);
        }
        catch
        {
            // Best effort.
        }
    }

    public static void ResetGridState(DataGrid grid)
    {
        if (grid is null)
            return;

        try
        {
            grid.CommitEdit(DataGridEditingUnit.Cell, true);
            grid.CommitEdit(DataGridEditingUnit.Row, true);
        }
        catch
        {
            // Best effort.
        }

        try
        {
            grid.SelectedItem = null;
            grid.UnselectAll();
            grid.UpdateLayout();
            grid.Focus();
        }
        catch
        {
            // Best effort.
        }
    }

    private static TChild? FindVisualChild<TChild>(DependencyObject dependencyObject)
        where TChild : DependencyObject
    {
        for (var index = 0; index < VisualTreeHelper.GetChildrenCount(dependencyObject); index++)
        {
            var child = VisualTreeHelper.GetChild(dependencyObject, index);

            if (child is TChild typedChild)
                return typedChild;

            var innerChild = FindVisualChild<TChild>(child);

            if (innerChild is not null)
                return innerChild;
        }

        return null;
    }
}