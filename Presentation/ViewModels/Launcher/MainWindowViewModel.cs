using System.Windows.Input;
using DukkanDepo.Infrastructure.Persistence.Backup;
using DukkanDepo.Presentation.Commands;
using DukkanDepo.Presentation.Services;

namespace DukkanDepo.Presentation.ViewModels.Launcher;

public sealed class MainWindowViewModel
{
    private readonly ILauncherNavigationService _navigationService;

    private readonly IFileDialogService _fileDialogService;

    private readonly IMessageService _messageService;

    private readonly IAppControlService _appControlService;

    public MainWindowViewModel(
        ILauncherNavigationService navigationService,
        IFileDialogService fileDialogService,
        IMessageService messageService,
        IAppControlService appControlService)
    {
        _navigationService = navigationService;
        _fileDialogService = fileDialogService;
        _messageService = messageService;
        _appControlService = appControlService;

        OpenYazlikCommand = new RelayCommand(_ => _navigationService.OpenYazlik());
        OpenKislikCommand = new RelayCommand(_ => _navigationService.OpenKislik());
        OpenAppearanceCommand = new RelayCommand(_ => _navigationService.OpenAppearance());

        BackupCommand = new AsyncRelayCommand(BackupAsync);
        RestoreCommand = new AsyncRelayCommand(RestoreAsync);
    }

    public ICommand OpenYazlikCommand { get; }

    public ICommand OpenKislikCommand { get; }

    public ICommand OpenAppearanceCommand { get; }

    public ICommand BackupCommand { get; }

    public ICommand RestoreCommand { get; }

    private async Task BackupAsync()
    {
        try
        {
            var path = _fileDialogService.PickSaveFile(
                title: "Yedek Al",
                filter: "Zip Yedeği (*.zip)|*.zip|SQLite Veritabanı (*.db)|*.db",
                fileName: $"backup_{DateTime.Now:yyyyMMdd}.zip",
                defaultExt: ".zip");

            if (string.IsNullOrWhiteSpace(path))
                return;

            var finalPath = await BackupService.CreateBackupToTargetPathAsync(path);

            BackupService.WaitUntilReadable(
                finalPath,
                timeoutMs: 6000);

            _messageService.Info($"Yedek oluşturuldu:\n{finalPath}");
        }
        catch (Exception exception)
        {
            _messageService.Error("Yedek alınamadı:\n" + exception.Message);
        }
    }

    private async Task RestoreAsync()
    {
        try
        {
            var path = _fileDialogService.PickOpenFile(
                title: "Yedekten Yükle",
                filter: "Tüm Yedekler (*.zip;*.db)|*.zip;*.db|Zip (*.zip)|*.zip|Veritabanı (*.db)|*.db");

            if (string.IsNullOrWhiteSpace(path))
                return;

            var confirmed = _messageService.Confirm(
                "Seçilen yedek mevcut veritabanının ÜSTÜNE yazılacak.\nDevam etmek istiyor musun?");

            if (!confirmed)
                return;

            await BackupService.RestoreBackupAsync(path);

            _messageService.Info("Yedek yüklendi. Uygulama yeniden başlatılacak.");

            _appControlService.RestartApp();
        }
        catch (Exception exception)
        {
            _messageService.Error("Yedekten yükleme başarısız:\n" + exception.Message);
        }
    }
}