using ClosedXML.Excel;
using DukkanDepo.Domain.Entities;

namespace DukkanDepo.Presentation.Services.Exports;

public static class UrunExcelExportService
{
    public static void Export<TProduct>(
        IFileDialogService fileDialogService,
        IMessageService messageService,
        IEnumerable<TProduct>? items,
        string filePrefix,
        string sheetName)
        where TProduct : Urun
    {
        var filePath = fileDialogService.PickSaveFile(
            title: "Excel olarak dışa aktar",
            filter: "Excel (*.xlsx)|*.xlsx",
            fileName: $"{filePrefix}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx",
            defaultExt: ".xlsx");

        if (string.IsNullOrWhiteSpace(filePath))
            return;

        try
        {
            ExportToFile(
                items,
                filePath,
                sheetName);

            messageService.Info("Excel'e aktarıldı.");
        }
        catch (Exception exception)
        {
            messageService.Error("Excel'e aktarılamadı:\n" + exception.Message);
        }
    }

    public static void ExportToFile<TProduct>(
        IEnumerable<TProduct>? items,
        string filePath,
        string sheetName)
        where TProduct : Urun
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(sheetName);

        string[] headers =
        [
            "Kod",
            "Cins",
            "Model",
            "Adet",
            "Satış",
            "Tutar",
            "Iskonto",
            "IskontoluTutar",
            "Tarih"
        ];

        for (var index = 0; index < headers.Length; index++)
        {
            worksheet.Cell(1, index + 1).Value = headers[index];
        }

        var row = 2;

        foreach (var product in items ?? Enumerable.Empty<TProduct>())
        {
            worksheet.Cell(row, 1).Value = product.Kod ?? string.Empty;
            worksheet.Cell(row, 2).Value = product.Cins ?? string.Empty;
            worksheet.Cell(row, 3).Value = product.Model ?? string.Empty;
            worksheet.Cell(row, 4).Value = product.Adet ?? 0;
            worksheet.Cell(row, 5).Value = (double)(product.Satis ?? 0m);
            worksheet.Cell(row, 6).Value = (double)(product.Tutar ?? 0m);
            worksheet.Cell(row, 7).Value = (double)(product.Iskonto ?? 0m);
            worksheet.Cell(row, 8).Value = (double)(product.IskontoluTutar ?? 0m);
            worksheet.Cell(row, 9).Value = product.Tarih;

            row++;
        }

        var headerRange = worksheet.Range(1, 1, 1, headers.Length);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        worksheet.Columns().AdjustToContents();

        worksheet.Column(4).Style.NumberFormat.Format = "#,##0";
        worksheet.Column(5).Style.NumberFormat.Format = "#,##0.00";
        worksheet.Column(6).Style.NumberFormat.Format = "#,##0.00";
        worksheet.Column(7).Style.NumberFormat.Format = "#,##0.00";
        worksheet.Column(8).Style.NumberFormat.Format = "#,##0.00";
        worksheet.Column(9).Style.DateFormat.Format = "dd.MM.yyyy HH:mm";

        workbook.SaveAs(filePath);
    }
}