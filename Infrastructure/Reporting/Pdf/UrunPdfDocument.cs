using System.Globalization;
using DukkanDepo.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

using QuestPdfContainer = QuestPDF.Infrastructure.IContainer;

namespace DukkanDepo.Infrastructure.Reporting.Pdf;

internal static class UrunPdfDocument
{
    public static IDocument Build(
        List<Urun> items,
        string title,
        CultureInfo culture)
    {
        return QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(24);
                page.DefaultTextStyle(text => text.FontSize(10));

                page.Header().Element(container =>
                    container.Row(row =>
                    {
                        row.RelativeItem()
                            .Text(title)
                            .SemiBold()
                            .FontSize(14);

                        row.ConstantItem(150)
                            .AlignRight()
                            .Text(DateTime.Now.ToString("dd.MM.yyyy HH:mm", culture));
                    }));

                page.Content().Element(container =>
                    BuildTable(container, items, culture));

                page.Footer()
                    .AlignRight()
                    .Text(text =>
                    {
                        text.Span("Sayfa ");
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
            });
        });
    }

    private static void BuildTable(
        QuestPdfContainer container,
        List<Urun> items,
        CultureInfo culture)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(1.0f); // Kod
                columns.RelativeColumn(1.2f); // Cins
                columns.RelativeColumn(2.0f); // Model
                columns.RelativeColumn(0.8f); // Adet
                columns.RelativeColumn(1.0f); // Satış
                columns.RelativeColumn(1.0f); // Tutar
                columns.RelativeColumn(1.0f); // İskonto
                columns.RelativeColumn(1.0f); // İskontolu Tutar
                columns.RelativeColumn(1.2f); // Tarih
            });

            table.Header(header =>
            {
                header.Cell().Element(PdfTableStyles.HeaderCell)
                    .Text(text => text.Span("Kod").Bold());

                header.Cell().Element(PdfTableStyles.HeaderCell)
                    .Text(text => text.Span("Cins").Bold());

                header.Cell().Element(PdfTableStyles.HeaderCell)
                    .Text(text => text.Span("Model").Bold());

                header.Cell().Element(PdfTableStyles.HeaderCell).AlignCenter()
                    .Text(text => text.Span("Adet").Bold());

                header.Cell().Element(PdfTableStyles.HeaderCell).AlignRight()
                    .Text(text => text.Span("Satış").Bold());

                header.Cell().Element(PdfTableStyles.HeaderCell).AlignRight()
                    .Text(text => text.Span("Tutar").Bold());

                header.Cell().Element(PdfTableStyles.HeaderCell).AlignRight()
                    .Text(text => text.Span("İskonto").Bold());

                header.Cell().Element(PdfTableStyles.HeaderCell).AlignRight()
                    .Text(text => text.Span("İskontolu Tutar").Bold());

                header.Cell().Element(PdfTableStyles.HeaderCell).AlignCenter()
                    .Text(text => text.Span("Tarih").Bold());
            });

            foreach (var product in items)
            {
                table.Cell().Element(PdfTableStyles.Cell)
                    .Text(product.Kod ?? string.Empty);

                table.Cell().Element(PdfTableStyles.Cell)
                    .Text(product.Cins ?? string.Empty);

                table.Cell().Element(PdfTableStyles.Cell)
                    .Text(product.Model ?? string.Empty);

                table.Cell().Element(PdfTableStyles.Cell).AlignCenter()
                    .Text((product.Adet ?? 0).ToString(culture));

                table.Cell().Element(PdfTableStyles.Cell).AlignRight()
                    .Text(PdfFormatters.FormatMoney(product.Satis, culture));

                table.Cell().Element(PdfTableStyles.Cell).AlignRight()
                    .Text(PdfFormatters.FormatMoney(product.Tutar, culture));

                table.Cell().Element(PdfTableStyles.Cell).AlignRight()
                    .Text(PdfFormatters.FormatMoney(product.Iskonto, culture));

                table.Cell().Element(PdfTableStyles.Cell).AlignRight()
                    .Text(PdfFormatters.FormatMoney(product.IskontoluTutar, culture));

                table.Cell().Element(PdfTableStyles.Cell).AlignCenter()
                    .Text(PdfFormatters.FormatDate(product.Tarih, culture));
            }

            AddTotalsRow(table, items, culture);
        });
    }

    private static void AddTotalsRow(
        TableDescriptor table,
        List<Urun> items,
        CultureInfo culture)
    {
        var toplamAdet = items.Sum(product => product.Adet ?? 0);
        var toplamTutar = items.Sum(product => product.Tutar ?? 0m);
        var toplamIskontoluTutar = items.Sum(product => product.IskontoluTutar ?? 0m);

        table.Cell()
            .ColumnSpan(3)
            .Element(PdfTableStyles.Cell)
            .Text(text => text.Span("TOPLAM").Bold());

        table.Cell()
            .Element(PdfTableStyles.Cell)
            .AlignCenter()
            .Text(toplamAdet.ToString("N0", culture));

        table.Cell()
            .Element(PdfTableStyles.Cell)
            .AlignRight()
            .Text(string.Empty);

        table.Cell()
            .Element(PdfTableStyles.Cell)
            .AlignRight()
            .Text(PdfFormatters.FormatMoney(toplamTutar, culture));

        table.Cell()
            .Element(PdfTableStyles.Cell)
            .AlignRight()
            .Text(string.Empty);

        table.Cell()
            .Element(PdfTableStyles.Cell)
            .AlignRight()
            .Text(PdfFormatters.FormatMoney(toplamIskontoluTutar, culture));

        table.Cell()
            .Element(PdfTableStyles.Cell)
            .Text(string.Empty);
    }
}