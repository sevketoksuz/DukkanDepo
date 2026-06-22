using System.Windows;
using DukkanDepo.Application.Abstractions.Charts;
using DukkanDepo.Presentation.ViewModels.Charts;

namespace DukkanDepo.Presentation.Views;

public partial class ChartsWindow : Window
{
    public ChartsWindow(IChartsDataProvider provider)
    {
        InitializeComponent();

        DataContext = new ChartsViewModel(provider);
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}