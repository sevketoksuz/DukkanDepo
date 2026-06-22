using System.Globalization;
using DukkanDepo.Application.Common.Comparers;
using DukkanDepo.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace DukkanDepo.Infrastructure.Reporting.Pdf;

public static class PdfService
{
    public static void GeneratePdfToFile(
        IEnumerable<Urun>? products,
        string title,
        string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        QuestPDF.Settings.License = LicenseType.Community;

        var comparer = new KodNaturalComparer();

        var list = (products ?? Enumerable.Empty<Urun>())
            .OrderBy(product => product.Kod ?? string.Empty, comparer)
            .ToList();

        var culture = CultureInfo.GetCultureInfo("tr-TR");

        var document = UrunPdfDocument.Build(list, title, culture);

        document.GeneratePdf(filePath);
    }
}