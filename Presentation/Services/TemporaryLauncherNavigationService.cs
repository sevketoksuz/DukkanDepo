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
        _messageService.Info("Yazlık ürün ekranı sonraki aşamada eklenecek.");
    }

    public void OpenKislik()
    {
        _messageService.Info("Kışlık ürün ekranı sonraki aşamada eklenecek.");
    }

    public void OpenAppearance()
    {
        var window = new AppearanceWindow();
        window.ShowDialog();
    }
}