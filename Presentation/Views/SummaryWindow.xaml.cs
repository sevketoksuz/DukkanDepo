using System.Windows;
using DukkanDepo.Application.Abstractions.Summary;
using DukkanDepo.Presentation.ViewModels.Summary;

namespace DukkanDepo.Presentation.Views;

public partial class SummaryWindow : Window
{
    public SummaryWindow(ISummaryDataProvider provider)
    {
        InitializeComponent();

        DataContext = new SummaryViewModel(provider);
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}