namespace DukkanDepo.Domain.Entities;

public abstract class Urun
{
    public int Id { get; set; }

    public string? Kod { get; set; }
    public string? Model { get; set; }
    public string? Cins { get; set; }

    public decimal? Iskonto { get; set; }
    public int? Adet { get; set; }
    public decimal? Satis { get; set; }

    public decimal? Tutar { get; set; }
    public decimal? IskontoluTutar { get; set; }

    public DateTime Tarih { get; set; } = DateTime.Now;

    public void Recalculate()
    {
        var satis = Satis ?? 0m;
        var iskonto = Iskonto ?? 0m;
        var adet = Adet ?? 0;

        Tutar = satis * adet;
        IskontoluTutar = (satis - iskonto) * adet;
    }
}