using DukkanDepo.Presentation.Views;

namespace DukkanDepo.Presentation.Services;

public sealed class TemporaryLauncherNavigationService : ILauncherNavigationService
{
    private readonly IMessageService _messageService;

    public TemporaryLauncherNavigationService(IMessageService messageService)
    {
        _messageService = messageService;
    }

    public void OpenYazlik()
    {
        var window = new YazlikWindow();
        window.Show();
    }

    public void OpenKislik()
    {
        var window = new KislikWindow();
        window.Show();
    }

    public void OpenAppearance()
    {
        var window = new AppearanceWindow();
        window.ShowDialog();
    }
}