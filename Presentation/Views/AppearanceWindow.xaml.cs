using System.Windows;
using DukkanDepo.Presentation.ViewModels.Appearance;

namespace DukkanDepo.Presentation.Views;

public partial class AppearanceWindow : Window
{
    public AppearanceWindow()
    {
        InitializeComponent();

        var viewModel = new AppearanceWindowViewModel();

        viewModel.RequestClose += Close;

        viewModel.RequestMessage += (message, title) =>
        {
            MessageBox.Show(
                this,
                message,
                title,
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        };

        DataContext = viewModel;
    }
}