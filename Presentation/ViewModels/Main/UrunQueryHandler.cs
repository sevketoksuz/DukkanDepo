using System.Globalization;
using DukkanDepo.Domain.Entities;

namespace DukkanDepo.Presentation.ViewModels.Main;

internal static class UrunQueryHelper
{
    public static bool TryParseDecimalFlexible(
        string? input,
        CultureInfo culture,
        out decimal value)
    {
        value = 0m;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        var text = input.Trim();
        var decimalSeparator = culture.NumberFormat.NumberDecimalSeparator;

        text = text
            .Replace(",", decimalSeparator)
            .Replace(".", decimalSeparator);

        if (text.EndsWith(decimalSeparator, StringComparison.Ordinal))
            text = text[..^decimalSeparator.Length];

        if (string.IsNullOrWhiteSpace(text))
            return false;

        if (decimal.TryParse(text, NumberStyles.Any, culture, out var parsed))
        {
            value = parsed;
            return true;
        }

        if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out parsed))
        {
            value = parsed;
            return true;
        }

        return false;
    }

    public static IQueryable<TProduct> ApplyDbFilters<TProduct>(
        IQueryable<TProduct> query,
        UrunFilterState filters,
        CultureInfo culture)
        where TProduct : Urun
    {
        if (!string.IsNullOrWhiteSpace(filters.Adet) &&
            int.TryParse(filters.Adet, out var adetDegeri))
        {
            query = query.Where(product => product.Adet == adetDegeri);
        }

        if (TryParseDecimalFlexible(filters.IskontoMin, culture, out var iskontoMin))
        {
            query = query.Where(product =>
                (double)(product.Iskonto ?? 0m) >= (double)iskontoMin);
        }

        if (TryParseDecimalFlexible(filters.IskontoMax, culture, out var iskontoMax))
        {
            query = query.Where(product =>
                (double)(product.Iskonto ?? 0m) <= (double)iskontoMax);
        }

        if (TryParseDecimalFlexible(filters.SatisMin, culture, out var satisMin))
        {
            query = query.Where(product =>
                (double)(product.Satis ?? 0m) >= (double)satisMin);
        }

        if (TryParseDecimalFlexible(filters.SatisMax, culture, out var satisMax))
        {
            query = query.Where(product =>
                (double)(product.Satis ?? 0m) <= (double)satisMax);
        }

        if (filters.TarihBaslangic.HasValue)
        {
            query = query.Where(product =>
                product.Tarih >= filters.TarihBaslangic.Value);
        }

        if (filters.TarihBitis.HasValue)
        {
            var end = filters.TarihBitis.Value.Date
                .AddDays(1)
                .AddTicks(-1);

            query = query.Where(product => product.Tarih <= end);
        }

        return query;
    }
}