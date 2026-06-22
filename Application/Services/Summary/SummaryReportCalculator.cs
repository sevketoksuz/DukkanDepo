using DukkanDepo.Application.Models;
using DukkanDepo.Domain.Entities;

namespace DukkanDepo.Application.Services.Summary;

public static class SummaryReportCalculator
{
    public static SummaryReport Calculate(
        IQueryable<Urun> query,
        DateTime now)
    {
        var data = query.ToList();

        var toplamAdet = data.Sum(x => x.Adet ?? 0);
        var toplamTutar = data.Sum(x => x.Tutar ?? 0m);
        var toplamIskontoluTutar = data.Sum(x => x.IskontoluTutar ?? 0m);
        var urunCesidi = data.Count;

        var son30GunTutar = data
            .Where(x => x.Tarih >= now.AddDays(-30))
            .Sum(x => x.Tutar ?? 0m);

        var son7GunIskontoluTutar = data
            .Where(x => x.Tarih >= now.AddDays(-7))
            .Sum(x => x.IskontoluTutar ?? 0m);

        var maxAdetUrun = data
            .OrderByDescending(x => x.Adet ?? 0)
            .FirstOrDefault();

        var enCokAdetKodModel = maxAdetUrun is null
            ? "-"
            : $"{maxAdetUrun.Kod} - {maxAdetUrun.Model} ({maxAdetUrun.Adet ?? 0})";

        var marjlar = data
            .Where(x => (x.Satis ?? 0m) != 0m)
            .Select(x =>
            {
                var satis = x.Satis ?? 0m;
                var iskonto = x.Iskonto ?? 0m;

                return ((satis - iskonto) / satis) * 100m;
            })
            .ToList();

        var ortalamaKarMarji = marjlar.Count == 0
            ? 0m
            : marjlar.Average();

        return new SummaryReport
        {
            ToplamAdet = toplamAdet,
            ToplamTutar = toplamTutar,
            ToplamIskontoluTutar = toplamIskontoluTutar,
            UrunCesidi = urunCesidi,
            Son30GunTutar = son30GunTutar,
            Son7GunIskontoluTutar = son7GunIskontoluTutar,
            EnCokAdetKodModel = enCokAdetKodModel,
            OrtalamaKarMarji = ortalamaKarMarji
        };
    }
}