using DukkanDepo.Application.Abstractions.Summary;

namespace DukkanDepo.Presentation.ViewModels.Summary;

public sealed class SummaryViewModel
{
    public int ToplamAdet { get; }

    public decimal ToplamTutar { get; }

    public decimal ToplamIskontoluTutar { get; }

    public int UrunCesidi { get; }

    public decimal Son30GunTutar { get; }

    public decimal Son7GunIskontoluTutar { get; }

    public string EnCokAdetKodModel { get; }

    public decimal OrtalamaKarMarji { get; }

    public SummaryViewModel(ISummaryDataProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);

        var report = provider.GetReport();

        ToplamAdet = report.ToplamAdet;
        ToplamTutar = report.ToplamTutar;
        ToplamIskontoluTutar = report.ToplamIskontoluTutar;
        UrunCesidi = report.UrunCesidi;
        Son30GunTutar = report.Son30GunTutar;
        Son7GunIskontoluTutar = report.Son7GunIskontoluTutar;
        EnCokAdetKodModel = report.EnCokAdetKodModel;
        OrtalamaKarMarji = report.OrtalamaKarMarji;
    }
}