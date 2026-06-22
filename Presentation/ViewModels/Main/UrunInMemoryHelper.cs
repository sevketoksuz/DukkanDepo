using System.ComponentModel;
using DukkanDepo.Domain.Entities;

namespace DukkanDepo.Presentation.ViewModels.Main;

internal static class UrunInMemoryHelper
{
    private static bool ContainsCi(string? haystack, string? needle)
    {
        if (string.IsNullOrWhiteSpace(needle))
            return true;

        if (string.IsNullOrEmpty(haystack))
            return false;

        return haystack.IndexOf(
            needle,
            StringComparison.CurrentCultureIgnoreCase) >= 0;
    }

    public static List<TProduct> ApplyTextFilters<TProduct>(
        List<TProduct> list,
        UrunFilterState filters)
        where TProduct : Urun
    {
        if (!string.IsNullOrWhiteSpace(filters.Kod))
        {
            list = list
                .Where(product => ContainsCi(product.Kod, filters.Kod))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(filters.Model))
        {
            list = list
                .Where(product => ContainsCi(product.Model, filters.Model))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(filters.Cins))
        {
            list = list
                .Where(product => ContainsCi(product.Cins, filters.Cins))
                .ToList();
        }

        return list;
    }

    public static List<TProduct> ApplySort<TProduct>(
        List<TProduct> list,
        string sortColumn,
        ListSortDirection sortDirection)
        where TProduct : Urun
    {
        var descending = sortDirection == ListSortDirection.Descending;

        return sortColumn switch
        {
            "Iskonto" => descending
                ? list.OrderByDescending(product => product.Iskonto ?? 0m).ToList()
                : list.OrderBy(product => product.Iskonto ?? 0m).ToList(),

            "Satis" => descending
                ? list.OrderByDescending(product => product.Satis ?? 0m).ToList()
                : list.OrderBy(product => product.Satis ?? 0m).ToList(),

            "Tutar" => descending
                ? list.OrderByDescending(product => product.Tutar ?? 0m).ToList()
                : list.OrderBy(product => product.Tutar ?? 0m).ToList(),

            "IskontoluTutar" => descending
                ? list.OrderByDescending(product => product.IskontoluTutar ?? 0m).ToList()
                : list.OrderBy(product => product.IskontoluTutar ?? 0m).ToList(),

            "Adet" => descending
                ? list.OrderByDescending(product => product.Adet.HasValue)
                    .ThenByDescending(product => product.Adet)
                    .ToList()
                : list.OrderByDescending(product => product.Adet.HasValue)
                    .ThenBy(product => product.Adet)
                    .ToList(),

            "Kod" => descending
                ? list.OrderByDescending(product => product.Kod).ToList()
                : list.OrderBy(product => product.Kod).ToList(),

            "Model" => descending
                ? list.OrderByDescending(product => product.Model).ToList()
                : list.OrderBy(product => product.Model).ToList(),

            "Cins" => descending
                ? list.OrderByDescending(product => product.Cins).ToList()
                : list.OrderBy(product => product.Cins).ToList(),

            "Tarih" => descending
                ? list.OrderByDescending(product => product.Tarih).ToList()
                : list.OrderBy(product => product.Tarih).ToList(),

            _ => descending
                ? list.OrderByDescending(product => product.Id).ToList()
                : list.OrderBy(product => product.Id).ToList()
        };
    }
}