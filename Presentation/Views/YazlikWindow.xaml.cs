using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using DukkanDepo.Domain.Entities;
using DukkanDepo.Infrastructure.Persistence.Context;
using DukkanDepo.Infrastructure.Persistence.Repositories;
using DukkanDepo.Infrastructure.Reporting.Charts;
using DukkanDepo.Infrastructure.Reporting.Summary;
using DukkanDepo.Presentation.Helpers;
using DukkanDepo.Presentation.Services;
using DukkanDepo.Presentation.Services.Exports;
using DukkanDepo.Presentation.ViewModels.Main;

namespace DukkanDepo.Presentation.Views;

public partial class YazlikWindow : Window
{
    private readonly AppDbContext _db;
    private readonly YazlikWindowViewModel _viewModel;
    private readonly IFileDialogService _fileDialogService;
    private readonly IMessageService _messageService;

    private string _currentSortColumn = "Id";
    private ListSortDirection _currentSortDirection = ListSortDirection.Descending;

    private bool _rowEditLock;
    private bool _suppressKeyHandler;
    private string? _editingOriginalText;

    public YazlikWindow()
    {
        InitializeComponent();

        _db = new AppDbContext();

        var repository = new UrunRepository<YazlikUrun>(_db);
        _viewModel = new YazlikWindowViewModel(repository);

        _fileDialogService = new WpfFileDialogService(this);
        _messageService = new WpfMessageService(this);

        DataContext = _viewModel;

        _viewModel.BeforeReload = () =>
            DataGridEditHelper.CommitAllEdits(UrunDataGrid);
    }

    private void Window_Closing(object? sender, CancelEventArgs e)
    {
        try
        {
            DataGridEditHelper.CommitAllEdits(UrunDataGrid);

            if (!DataGridEditHelper.IsPlaceholder(UrunDataGrid.CurrentItem) &&
                UrunDataGrid.CurrentItem is YazlikUrun product &&
                product.Id == 0 &&
                !string.IsNullOrWhiteSpace(product.Kod))
            {
                _viewModel.SaveRow(product);
            }
        }
        finally
        {
            _db.Dispose();
        }
    }

    private void UrunDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
    {
        e.Handled = true;

        _currentSortDirection =
            _currentSortColumn == e.Column.SortMemberPath &&
            _currentSortDirection == ListSortDirection.Ascending
                ? ListSortDirection.Descending
                : ListSortDirection.Ascending;

        _currentSortColumn = e.Column.SortMemberPath ?? "Id";
        e.Column.SortDirection = _currentSortDirection;

        _viewModel.VerileriYukle(
            _currentSortColumn,
            _currentSortDirection);
    }

    private void DataGrid_CellEditEnding(
        object sender,
        DataGridCellEditEndingEventArgs e)
    {
        if (e.EditAction != DataGridEditAction.Commit)
            return;

        if (e.EditingElement is TextBox textBox)
            textBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
    }

    private void UrunDataGrid_PreparingCellForEdit(
        object sender,
        DataGridPreparingCellForEditEventArgs e)
    {
        if (e.EditingElement is not TextBox textBox)
            return;

        _editingOriginalText = textBox.Text;

        var initiatedByTyping = e.EditingEventArgs is TextCompositionEventArgs;

        DataGridEditHelper.PrepareTextBoxForEdit(
            textBox,
            initiatedByTyping);
    }

    private void DataGrid_RowEditEnding(
        object sender,
        DataGridRowEditEndingEventArgs e)
    {
        if (_rowEditLock)
            return;

        if (e.EditAction == DataGridEditAction.Cancel)
            return;

        if (DataGridEditHelper.IsPlaceholder(e.Row?.Item))
            return;

        if (e.EditAction != DataGridEditAction.Commit)
            return;

        _rowEditLock = true;

        var grid = (DataGrid)sender;

        grid.CommitEdit(DataGridEditingUnit.Row, true);
        grid.CommitEdit(DataGridEditingUnit.Cell, true);

        Dispatcher.InvokeAsync(() =>
        {
            try
            {
                _suppressKeyHandler = true;

                if (e.Row?.Item is YazlikUrun product)
                    _viewModel.SaveRow(product);
            }
            finally
            {
                _suppressKeyHandler = false;
                _rowEditLock = false;
            }
        });
    }

    private void UrunDataGrid_CurrentCellChanged(object? sender, EventArgs e)
    {
        if (Keyboard.IsKeyDown(Key.Escape))
            return;
    }

    private async void Delete_Click(object sender, RoutedEventArgs e)
    {
        await DeleteSelectedAsync();
    }

    private void ExportExcel_Click(object? sender, RoutedEventArgs? e)
    {
        var items = _viewModel
            .UrunlerView
            .Cast<object>()
            .OfType<YazlikUrun>()
            .ToList();

        UrunExcelExportService.Export(
            _fileDialogService,
            _messageService,
            items,
            "YazlikUrunler",
            "Yazlik");
    }

    private void ImportExcel_Click(object? sender, RoutedEventArgs? e)
    {
        if (UrunExcelImportService.Import<YazlikUrun>(
                _fileDialogService,
                _messageService,
                out _))
        {
            _viewModel.VerileriYukle(
                _currentSortColumn,
                _currentSortDirection);
        }
    }

    private void ExportPdf_Click(object? sender, RoutedEventArgs? e)
    {
        var all = _viewModel.GetAllUrunNoTracking();

        UrunPdfExportService.Export(
            _fileDialogService,
            _messageService,
            all,
            "Yazlık Ürünler - Tüm Kayıtlar",
            "YazlikUrunler_TUM");
    }

    private void OpenSummary_Click(object sender, RoutedEventArgs e)
    {
        var provider = new EfSummaryDataProvider(() =>
            _viewModel.UrunQuery());

        new SummaryWindow(provider)
        {
            Owner = this
        }.ShowDialog();
    }

    private void OpenCharts_Click(object sender, RoutedEventArgs e)
    {
        var provider = new EfChartsDataProvider(() =>
            _viewModel.UrunQuery());

        new ChartsWindow(provider)
        {
            Owner = this
        }.ShowDialog();
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        var ctrl = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        var typing = Keyboard.FocusedElement is TextBox;

        if (e.Key == Key.F5)
        {
            _viewModel.VerileriYukle(
                _currentSortColumn,
                _currentSortDirection);

            e.Handled = true;
        }
        else if (ctrl && e.Key == Key.T && !typing)
        {
            _viewModel.TemizleFiltreCommand.Execute(null);
            e.Handled = true;
        }
        else if (ctrl && e.Key == Key.E && !typing)
        {
            ExportExcel_Click(null, null);
            e.Handled = true;
        }
        else if (ctrl && e.Key == Key.I && !typing)
        {
            ImportExcel_Click(null, null);
            e.Handled = true;
        }
    }

    private async void UrunDataGrid_PreviewKeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (_suppressKeyHandler)
            return;

        var grid = (DataGrid)sender;

        var currentCell = grid.CurrentCell;
        var currentColumn = currentCell.Column;
        var currentItem = currentCell.Item;

        var placeholderOrNull =
            currentColumn is null ||
            currentItem is null ||
            DataGridEditHelper.IsPlaceholder(currentItem);

        if (!DataGridEditHelper.IsEditingCell(grid) && !placeholderOrNull)
        {
            var sortMemberPath = currentColumn!.SortMemberPath ?? string.Empty;

            if (DataGridTemplateEditHelper.IsNumericEditColumn(sortMemberPath))
            {
                var inputText = DataGridTemplateEditHelper.GetNumericTextFromKey(e.Key);

                if (inputText is not null &&
                    DataGridTemplateEditHelper.IsAllowedFirstInput(sortMemberPath, inputText))
                {
                    e.Handled = true;

                    await DataGridTemplateEditHelper.BeginEditAsync(
                        grid,
                        currentItem!,
                        currentColumn!,
                        inputText);

                    return;
                }
            }

            if (e.Key == Key.F2)
            {
                e.Handled = true;

                await DataGridTemplateEditHelper.BeginEditAsync(
                    grid,
                    currentItem!,
                    currentColumn!,
                    null);

                return;
            }
        }

        if (e.Key == Key.Escape)
        {
            if (DataGridEditHelper.IsEditingCell(grid))
            {
                e.Handled = true;

                var textBox = DataGridEditHelper.TryGetFocusedTextBox();

                if (textBox is not null && _editingOriginalText is not null)
                {
                    textBox.Text = _editingOriginalText;
                    textBox.CaretIndex = textBox.Text.Length;
                    textBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                }

                await Dispatcher.InvokeAsync(() =>
                {
                    try
                    {
                        _suppressKeyHandler = true;

                        grid.CommitEdit(DataGridEditingUnit.Cell, true);
                        grid.CommitEdit(DataGridEditingUnit.Row, true);
                    }
                    finally
                    {
                        _suppressKeyHandler = false;
                        _editingOriginalText = null;
                    }
                }, DispatcherPriority.Background);
            }

            return;
        }

        if (e.Key is Key.Return or Key.Enter)
        {
            if (DataGridEditHelper.IsEditingCell(grid))
            {
                e.Handled = true;

                DataGridEditHelper
                    .TryGetFocusedTextBox()
                    ?.GetBindingExpression(TextBox.TextProperty)
                    ?.UpdateSource();

                await Dispatcher.InvokeAsync(() =>
                {
                    try
                    {
                        _suppressKeyHandler = true;

                        grid.CommitEdit(DataGridEditingUnit.Cell, true);
                        grid.CommitEdit(DataGridEditingUnit.Row, true);
                    }
                    finally
                    {
                        _suppressKeyHandler = false;
                        _editingOriginalText = null;
                    }
                }, DispatcherPriority.Background);
            }

            return;
        }

        if (e.Key == Key.Delete &&
            !DataGridEditHelper.IsEditingCell(grid))
        {
            await DeleteSelectedAsync();
            e.Handled = true;
        }
    }

    private async Task DeleteSelectedAsync()
    {
        DataGridEditHelper.CommitAllEdits(UrunDataGrid);

        var selected = UrunDataGrid
            .SelectedItems
            .OfType<YazlikUrun>()
            .Where(product => product.Id > 0)
            .ToList();

        if (selected.Count == 0 &&
            _viewModel.SeciliUrun is not null &&
            _viewModel.SeciliUrun.Id > 0)
        {
            selected = [_viewModel.SeciliUrun];
        }

        if (selected.Count == 0)
            return;

        var confirmed = MessageBox.Show(
            this,
            selected.Count == 1
                ? "Seçili satır silinsin mi?"
                : $"{selected.Count} satır silinsin mi?",
            "Onay",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning) == MessageBoxResult.Yes;

        if (!confirmed)
            return;

        try
        {
            await _viewModel.DeleteManyByIdsAsync(
                selected.Select(product => product.Id));

            _viewModel.VerileriYukle(
                _currentSortColumn,
                _currentSortDirection);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                "Silme başarısız:\n" + exception.Message,
                "Hata",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}