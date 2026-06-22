using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DukkanDepo.Domain.Entities;

public abstract class Urun : INotifyPropertyChanged
{
    private int _id;
    private string? _kod;
    private string? _model;
    private string? _cins;
    private decimal? _iskonto;
    private int? _adet;
    private decimal? _satis;
    private decimal? _tutar;
    private decimal? _iskontoluTutar;
    private DateTime _tarih = DateTime.Now;

    public event PropertyChangedEventHandler? PropertyChanged;

    public int Id
    {
        get => _id;
        set => SetField(ref _id, value);
    }

    public string? Kod
    {
        get => _kod;
        set => SetField(ref _kod, value);
    }

    public string? Model
    {
        get => _model;
        set => SetField(ref _model, value);
    }

    public string? Cins
    {
        get => _cins;
        set => SetField(ref _cins, value);
    }

    public decimal? Iskonto
    {
        get => _iskonto;
        set
        {
            if (!SetField(ref _iskonto, value))
                return;

            Recalculate();
        }
    }

    public int? Adet
    {
        get => _adet;
        set
        {
            if (!SetField(ref _adet, value))
                return;

            Recalculate();
        }
    }

    public decimal? Satis
    {
        get => _satis;
        set
        {
            if (!SetField(ref _satis, value))
                return;

            Recalculate();
        }
    }

    public decimal? Tutar
    {
        get => _tutar;
        set => SetField(ref _tutar, value);
    }

    public decimal? IskontoluTutar
    {
        get => _iskontoluTutar;
        set => SetField(ref _iskontoluTutar, value);
    }

    public DateTime Tarih
    {
        get => _tarih;
        set => SetField(ref _tarih, value);
    }

    public void Recalculate()
    {
        var satis = Satis ?? 0m;
        var iskonto = Iskonto ?? 0m;
        var adet = Adet ?? 0;

        Tutar = satis * adet;
        IskontoluTutar = (satis - iskonto) * adet;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(
        ref T field,
        T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}