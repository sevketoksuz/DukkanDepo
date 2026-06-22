using System.Globalization;
using System.Threading;
using System.Windows;
using DukkanDepo.Infrastructure.Persistence.Backup;
using DukkanDepo.Infrastructure.Persistence.Context;
using DukkanDepo.Presentation.Services.Appearance;
using DukkanDepo.Presentation.Views;

namespace DukkanDepo;

public partial class App : System.Windows.Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        DispatcherUnhandledException += App_DispatcherUnhandledException;

        ApplyCulture();

        try
        {
            AppDbContext.EnsureMigrate();
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                "Veritabanı başlatma hatası:\n" + exception.Message,
                "DukkanDepo",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Shutdown();
            return;
        }

        AppearanceService.LoadAndApply();

        try
        {
            await BackupService.CreateBackupIfOlderThanAsync(
                TimeSpan.FromHours(24),
                asZip: true);

            BackupService.PruneBackups(
                keepLast: 30,
                keepDays: 60);
        }
        catch
        {
            // Başlangıç backup hatası uygulamayı düşürmesin.
        }

        var window = new MainWindow();
        window.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        try
        {
            await BackupService.CreateBackupIfOlderThanAsync(
                TimeSpan.FromHours(6),
                asZip: true);

            BackupService.PruneBackups(
                keepLast: 30,
                keepDays: 60);
        }
        catch
        {
            // Çıkış backup hatası uygulamayı düşürmesin.
        }

        base.OnExit(e);
    }

    private static void ApplyCulture()
    {
        var culture = new CultureInfo("tr-TR");

        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        // Startup işi OnStartup içinde yapılıyor.
    }

    private void App_DispatcherUnhandledException(
        object sender,
        System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        if (e.Exception is ArgumentException argumentException &&
            argumentException.Message.Contains("Visual or Visual3D"))
        {
            e.Handled = true;
        }
    }
}