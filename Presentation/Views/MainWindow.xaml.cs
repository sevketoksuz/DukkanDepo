using System.Windows;
using DukkanDepo.Presentation.Services;
using DukkanDepo.Presentation.ViewModels.Launcher;

namespace DukkanDepo.Presentation.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var messages = new WpfMessageService(this);
        var navigation = new TemporaryLauncherNavigationService(messages);
        var dialogs = new WpfFileDialogService(this);
        var appControl = new AppControlService();

        DataContext = new MainWindowViewModel(
            navigation,
            dialogs,
            messages,
            appControl);
    }
}