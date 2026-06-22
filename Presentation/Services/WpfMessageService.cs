using System.Windows;

namespace DukkanDepo.Presentation.Services;

public sealed class WpfMessageService : IMessageService
{
    private readonly Window _owner;

    public WpfMessageService(Window owner)
    {
        _owner = owner;
    }

    public void Info(string message, string title = "Bilgi")
    {
        MessageBox.Show(
            _owner,
            message,
            title,
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    public void Error(string message, string title = "Hata")
    {
        MessageBox.Show(
            _owner,
            message,
            title,
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }

    public bool Confirm(string message, string title = "Onay")
    {
        return MessageBox.Show(
            _owner,
            message,
            title,
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning) == MessageBoxResult.Yes;
    }
}