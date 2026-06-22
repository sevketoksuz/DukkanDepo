using System.Diagnostics;

namespace DukkanDepo.Presentation.Services;

public sealed class AppControlService : IAppControlService
{
    public void RestartApp()
    {
        try
        {
            var exePath = Process.GetCurrentProcess().MainModule?.FileName;

            if (!string.IsNullOrWhiteSpace(exePath))
            {
                Process.Start(new ProcessStartInfo(exePath)
                {
                    UseShellExecute = true
                });
            }
        }
        catch
        {
            // Restart başarısız olsa bile uygulama kapanabilir.
        }

        System.Windows.Application.Current.Shutdown();
    }
}