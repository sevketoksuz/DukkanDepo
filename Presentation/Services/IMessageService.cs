namespace DukkanDepo.Presentation.Services;

public interface IMessageService
{
    void Info(string message, string title = "Bilgi");

    void Error(string message, string title = "Hata");

    bool Confirm(string message, string title = "Onay");
}