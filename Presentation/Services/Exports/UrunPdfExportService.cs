using DukkanDepo.Domain.Entities;
using DukkanDepo.Infrastructure.Reporting.Pdf;

namespace DukkanDepo.Presentation.Services.Exports;

public static class UrunPdfExportService
{
    public static void Export(
        IFileDialogService fileDialogService,
        IMessageService messageService,
        IEnumerable<Urun>? products,
        string title,
        string filePrefix)
    {
        var filePath = fileDialogService.PickSaveFile(
            title: "PDF olarak dışa aktar",
            filter: "PDF (*.pdf)|*.pdf",
            fileName: $"{filePrefix}_{DateTime.Now:yyyyMMdd_HHmm}.pdf",
            defaultExt: ".pdf");

        if (string.IsNullOrWhiteSpace(filePath))
            return;

        var list = (products ?? Enumerable.Empty<Urun>()).ToList();

        if (list.Count == 0)
        {
            messageService.Info("Dışa aktarılacak kayıt yok.");
            return;
        }

        try
        {
            PdfService.GeneratePdfToFile(
                list,
                title,
                filePath);

            messageService.Info("PDF oluşturuldu.");
        }
        catch (Exception exception)
        {
            messageService.Error("PDF oluşturulamadı:\n" + exception.Message);
        }
    }
}