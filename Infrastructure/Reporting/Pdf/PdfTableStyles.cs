using QuestPDF.Fluent;
using QuestPDF.Helpers;

using QuestPdfContainer = QuestPDF.Infrastructure.IContainer;

namespace DukkanDepo.Infrastructure.Reporting.Pdf;

internal static class PdfTableStyles
{
    public static QuestPdfContainer Cell(QuestPdfContainer container)
    {
        return container
            .BorderBottom(0.5f)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingVertical(4)
            .PaddingHorizontal(4);
    }

    public static QuestPdfContainer HeaderCell(QuestPdfContainer container)
    {
        return container
            .Background(Colors.Grey.Lighten3)
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Medium)
            .PaddingVertical(6)
            .PaddingHorizontal(4);
    }
}