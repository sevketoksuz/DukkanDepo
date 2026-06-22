namespace DukkanDepo.Presentation.ViewModels.Main;

internal sealed class UrunFilterState
{
    public string? Kod { get; set; }

    public string? Model { get; set; }

    public string? Cins { get; set; }

    public string? Adet { get; set; }

    public string? IskontoMin { get; set; }

    public string? IskontoMax { get; set; }

    public string? SatisMin { get; set; }

    public string? SatisMax { get; set; }

    public DateTime? TarihBaslangic { get; set; }

    public DateTime? TarihBitis { get; set; }

    public void Clear()
    {
        Kod = null;
        Model = null;
        Cins = null;
        Adet = null;
        IskontoMin = null;
        IskontoMax = null;
        SatisMin = null;
        SatisMax = null;
        TarihBaslangic = null;
        TarihBitis = null;
    }
}