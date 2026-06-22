using ClosedXML.Excel;
using DukkanDepo.Domain.Entities;
using DukkanDepo.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.IO;

namespace DukkanDepo.Presentation.Services.Exports;

public static class UrunExcelImportService
{
    public static bool Import<TProduct>(
        IFileDialogService fileDialogService,
        IMessageService messageService,
        out ExcelImportResult result)
        where TProduct : Urun, new()
    {
        result = new ExcelImportResult();

        var filePath = fileDialogService.PickOpenFile(
            title: "Excel dosyası seç",
            filter: "Excel (*.xlsx)|*.xlsx");

        if (string.IsNullOrWhiteSpace(filePath))
            return false;

        try
        {
            result = ImportFromFile<TProduct>(filePath);

            messageService.Info(
                $"İçe aktarma tamamlandı.\n" +
                $"Eklendi: {result.Added}\n" +
                $"Güncellendi: {result.Updated}\n" +
                $"Atlanan: {result.Skipped}");

            return true;
        }
        catch (Exception exception)
        {
            messageService.Error("Excel içe aktarılamadı:\n" + exception.Message);
            return false;
        }
    }

    public static ExcelImportResult ImportFromFile<TProduct>(string filePath)
        where TProduct : Urun, new()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath))
            throw new FileNotFoundException("Excel dosyası bulunamadı.", filePath);

        var culture = CultureInfo.GetCultureInfo("tr-TR");

        using var db = new AppDbContext();
        using var workbook = new XLWorkbook(filePath);

        var worksheet = workbook.Worksheets.FirstOrDefault()
                        ?? throw new InvalidOperationException("Excel içinde çalışma sayfası bulunamadı.");

        var lastRowUsed = worksheet.LastRowUsed();

        if (lastRowUsed is null)
            throw new InvalidOperationException("Excel boş görünüyor.");

        var lastRow = lastRowUsed.RowNumber();
        var columnMap = BuildColumnMap(worksheet);
        var set = db.Set<TProduct>();

        var result = new ExcelImportResult();

        for (var row = 2; row <= lastRow; row++)
        {
            try
            {
                ImportRow(
                    worksheet,
                    row,
                    set,
                    columnMap,
                    result,
                    culture);
            }
            catch
            {
                result.Skipped++;
            }
        }

        db.SaveChanges();

        return result;
    }

    private static void ImportRow<TProduct>(
        IXLWorksheet worksheet,
        int row,
        DbSet<TProduct> set,
        ExcelColumnMap columnMap,
        ExcelImportResult result,
        CultureInfo culture)
        where TProduct : Urun, new()
    {
        var kod = ReadString(worksheet.Cell(row, columnMap.Kod));
        var cins = ReadString(worksheet.Cell(row, columnMap.Cins));
        var model = ReadString(worksheet.Cell(row, columnMap.Model));

        if (string.IsNullOrWhiteSpace(kod) &&
            string.IsNullOrWhiteSpace(model))
        {
            result.Skipped++;
            return;
        }

        var adet = TryParseInt(worksheet.Cell(row, columnMap.Adet));
        var satis = TryParseDecimal(worksheet.Cell(row, columnMap.Satis), culture);
        var iskonto = TryParseDecimal(worksheet.Cell(row, columnMap.Iskonto), culture);
        var tarih = TryParseDate(worksheet.Cell(row, columnMap.Tarih), culture);

        var kayit = set.FirstOrDefault(product =>
            product.Kod == kod &&
            product.Model == model);

        if (kayit is null)
        {
            kayit = new TProduct
            {
                Kod = string.IsNullOrWhiteSpace(kod) ? null : kod,
                Cins = string.IsNullOrWhiteSpace(cins) ? null : cins,
                Model = string.IsNullOrWhiteSpace(model) ? null : model,
                Adet = adet,
                Satis = satis,
                Iskonto = iskonto,
                Tarih = tarih ?? DateTime.Now
            };

            kayit.Recalculate();

            set.Add(kayit);
            result.Added++;

            return;
        }

        kayit.Cins = string.IsNullOrWhiteSpace(cins)
            ? kayit.Cins
            : cins;

        kayit.Adet = adet;
        kayit.Satis = satis;
        kayit.Iskonto = iskonto;

        if (tarih.HasValue)
            kayit.Tarih = tarih.Value;

        kayit.Recalculate();

        set.Update(kayit);
        result.Updated++;
    }

    private static ExcelColumnMap BuildColumnMap(IXLWorksheet worksheet)
    {
        var defaultMap = new ExcelColumnMap();

        var headerRow = worksheet.Row(1);
        var usedCells = headerRow.CellsUsed().ToList();

        if (usedCells.Count == 0)
            return defaultMap;

        var dictionary = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var cell in usedCells)
        {
            var name = NormalizeHeader(cell.GetString());

            if (string.IsNullOrWhiteSpace(name))
                continue;

            dictionary[name] = cell.Address.ColumnNumber;
        }

        int Find(params string[] keys)
        {
            foreach (var key in keys)
            {
                var normalizedKey = NormalizeHeader(key);

                if (dictionary.TryGetValue(normalizedKey, out var columnIndex))
                    return columnIndex;
            }

            return -1;
        }

        var kod = Find("Kod", "Ürün Kodu", "UrunKodu");
        var cins = Find("Cins");
        var model = Find("Model");
        var adet = Find("Adet");
        var satis = Find("Satış", "Satis");
        var iskonto = Find("Iskonto", "İskonto");
        var tarih = Find("Tarih", "Date");

        return new ExcelColumnMap
        {
            Kod = kod > 0 ? kod : defaultMap.Kod,
            Cins = cins > 0 ? cins : defaultMap.Cins,
            Model = model > 0 ? model : defaultMap.Model,
            Adet = adet > 0 ? adet : defaultMap.Adet,
            Satis = satis > 0 ? satis : defaultMap.Satis,
            Iskonto = iskonto > 0 ? iskonto : defaultMap.Iskonto,
            Tarih = tarih > 0 ? tarih : defaultMap.Tarih
        };
    }

    private static string NormalizeHeader(string value)
    {
        return (value ?? string.Empty)
            .Trim()
            .Replace(" ", string.Empty)
            .Replace("_", string.Empty)
            .Replace("-", string.Empty)
            .Replace("İ", "I")
            .Replace("ı", "i")
            .Replace("Ş", "S")
            .Replace("ş", "s")
            .Replace("Ğ", "G")
            .Replace("ğ", "g")
            .Replace("Ü", "U")
            .Replace("ü", "u")
            .Replace("Ö", "O")
            .Replace("ö", "o")
            .Replace("Ç", "C")
            .Replace("ç", "c");
    }

    private static string? ReadString(IXLCell cell)
    {
        var value = cell.GetString()?.Trim();

        return string.IsNullOrWhiteSpace(value)
            ? null
            : value;
    }

    private static decimal? TryParseDecimal(
        IXLCell cell,
        CultureInfo culture)
    {
        if (cell.IsEmpty())
            return null;

        try
        {
            return (decimal)cell.GetDouble();
        }
        catch
        {
            // String parse'a düş.
        }

        var text = cell.GetFormattedString()?.Trim();

        if (string.IsNullOrWhiteSpace(text))
            text = cell.GetString()?.Trim();

        if (string.IsNullOrWhiteSpace(text))
            return null;

        text = text
            .Replace("₺", string.Empty)
            .Replace("$", string.Empty)
            .Replace("€", string.Empty)
            .Replace(" ", string.Empty);

        if (decimal.TryParse(text, NumberStyles.Any, culture, out var decimalValue))
            return decimalValue;

        if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimalValue))
            return decimalValue;

        return null;
    }

    private static int? TryParseInt(IXLCell cell)
    {
        if (cell.IsEmpty())
            return null;

        try
        {
            return (int)Math.Round(cell.GetDouble());
        }
        catch
        {
            // String parse'a düş.
        }

        var text = cell.GetFormattedString()?.Trim();

        if (string.IsNullOrWhiteSpace(text))
            text = cell.GetString()?.Trim();

        return int.TryParse(text, out var value)
            ? value
            : null;
    }

    private static DateTime? TryParseDate(
        IXLCell cell,
        CultureInfo culture)
    {
        if (cell.IsEmpty())
            return null;

        try
        {
            return cell.GetDateTime();
        }
        catch
        {
            // String parse'a düş.
        }

        var text = cell.GetFormattedString()?.Trim();

        if (string.IsNullOrWhiteSpace(text))
            text = cell.GetString()?.Trim();

        if (string.IsNullOrWhiteSpace(text))
            return null;

        if (DateTime.TryParse(text, culture, DateTimeStyles.None, out var date))
            return date;

        if (DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            return date;

        if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var oaDate))
            return DateTime.FromOADate(oaDate);

        return null;
    }

    private sealed class ExcelColumnMap
    {
        public int Kod { get; init; } = 1;
        public int Cins { get; init; } = 2;
        public int Model { get; init; } = 3;
        public int Adet { get; init; } = 4;
        public int Satis { get; init; } = 5;
        public int Iskonto { get; init; } = 7;
        public int Tarih { get; init; } = 9;
    }
}