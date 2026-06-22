using System.Globalization;

namespace DukkanDepo.Infrastructure.Reporting.Pdf;

internal static class PdfFormatters
{
    public static string FormatMoney(decimal? value, CultureInfo culture)
    {
        return value.HasValue
            ? value.Value.ToString("N2", culture) + " $"
            : string.Empty;
    }

    public static string FormatDate(DateTime date, CultureInfo culture)
    {
        return date.ToString("dd.MM.yyyy HH:mm", culture);
    }
}