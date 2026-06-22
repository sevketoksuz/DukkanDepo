using System.Windows;
using Microsoft.Win32;

namespace DukkanDepo.Presentation.Services;

public sealed class WpfFileDialogService : IFileDialogService
{
    private readonly Window _owner;

    public WpfFileDialogService(Window owner)
    {
        _owner = owner;
    }

    public string? PickSaveFile(
        string title,
        string filter,
        string fileName,
        string? defaultExt = null)
    {
        var dialog = new SaveFileDialog
        {
            Title = title,
            Filter = filter,
            FileName = fileName,
            DefaultExt = defaultExt
        };

        return dialog.ShowDialog(_owner) == true
            ? dialog.FileName
            : null;
    }

    public string? PickOpenFile(
        string title,
        string filter)
    {
        var dialog = new OpenFileDialog
        {
            Title = title,
            Filter = filter
        };

        return dialog.ShowDialog(_owner) == true
            ? dialog.FileName
            : null;
    }
}