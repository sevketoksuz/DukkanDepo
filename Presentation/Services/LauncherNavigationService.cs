using System.Windows;
using DukkanDepo.Presentation.Views;

namespace DukkanDepo.Presentation.Services;

public sealed class LauncherNavigationService : ILauncherNavigationService
{
    private readonly Window _owner;

    private YazlikWindow? _yazlikWindow;
    private KislikWindow? _kislikWindow;

    public LauncherNavigationService(Window owner)
    {
        _owner = owner;
    }

    public void OpenYazlik()
    {
        if (_yazlikWindow is not null)
        {
            BringToFront(_yazlikWindow);
            return;
        }

        _yazlikWindow = new YazlikWindow
        {
            Owner = _owner
        };

        _yazlikWindow.Closed += (_, _) =>
        {
            _yazlikWindow = null;
        };

        _yazlikWindow.Show();
    }

    public void OpenKislik()
    {
        if (_kislikWindow is not null)
        {
            BringToFront(_kislikWindow);
            return;
        }

        _kislikWindow = new KislikWindow
        {
            Owner = _owner
        };

        _kislikWindow.Closed += (_, _) =>
        {
            _kislikWindow = null;
        };

        _kislikWindow.Show();
    }

    public void OpenAppearance()
    {
        var window = new AppearanceWindow
        {
            Owner = _owner
        };

        window.ShowDialog();
    }

    private static void BringToFront(Window window)
    {
        if (window.WindowState == WindowState.Minimized)
            window.WindowState = WindowState.Normal;

        window.Activate();
        window.Focus();
    }
}