namespace DukkanDepo.Application.Models;

public sealed class SummaryReport
{
    public int ToplamAdet { get; init; }
    public decimal ToplamTutar { get; init; }
    public decimal ToplamIskontoluTutar { get; init; }
    public int UrunCesidi { get; init; }
    public decimal Son30GunTutar { get; init; }
    public decimal Son7GunIskontoluTutar { get; init; }
    public string EnCokAdetKodModel { get; init; } = "-";
    public decimal OrtalamaKarMarji { get; init; }
}