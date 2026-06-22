using DukkanDepo.Domain.Entities;
using DukkanDepo.Presentation.Common;

namespace DukkanDepo.Presentation.ViewModels;

public class UrunRowViewModel<TProduct> : ObservableObject
    where TProduct : Urun
{
    public TProduct Model { get; }

    public UrunRowViewModel(TProduct model)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
    }

    public int Id
    {
        get => Model.Id;
        set
        {
            if (Model.Id == value)
                return;

            Model.Id = value;
            OnPropertyChanged();
        }
    }

    public string? Kod
    {
        get => Model.Kod;
        set
        {
            if (Model.Kod == value)
                return;

            Model.Kod = value;
            OnPropertyChanged();
        }
    }

    public string? ModelAdi
    {
        get => Model.Model;
        set
        {
            if (Model.Model == value)
                return;

            Model.Model = value;
            OnPropertyChanged();
        }
    }

    public string? Cins
    {
        get => Model.Cins;
        set
        {
            if (Model.Cins == value)
                return;

            Model.Cins = value;
            OnPropertyChanged();
        }
    }

    public decimal? Iskonto
    {
        get => Model.Iskonto;
        set
        {
            if (Model.Iskonto == value)
                return;

            Model.Iskonto = value;
            Model.Recalculate();

            OnPropertyChanged();
            OnPropertyChanged(nameof(Tutar));
            OnPropertyChanged(nameof(IskontoluTutar));
        }
    }

    public int? Adet
    {
        get => Model.Adet;
        set
        {
            if (Model.Adet == value)
                return;

            Model.Adet = value;
            Model.Recalculate();

            OnPropertyChanged();
            OnPropertyChanged(nameof(Tutar));
            OnPropertyChanged(nameof(IskontoluTutar));
        }
    }

    public decimal? Satis
    {
        get => Model.Satis;
        set
        {
            if (Model.Satis == value)
                return;

            Model.Satis = value;
            Model.Recalculate();

            OnPropertyChanged();
            OnPropertyChanged(nameof(Tutar));
            OnPropertyChanged(nameof(IskontoluTutar));
        }
    }

    public decimal? Tutar
    {
        get => Model.Tutar;
        set
        {
            if (Model.Tutar == value)
                return;

            Model.Tutar = value;
            OnPropertyChanged();
        }
    }

    public decimal? IskontoluTutar
    {
        get => Model.IskontoluTutar;
        set
        {
            if (Model.IskontoluTutar == value)
                return;

            Model.IskontoluTutar = value;
            OnPropertyChanged();
        }
    }

    public DateTime Tarih
    {
        get => Model.Tarih;
        set
        {
            if (Model.Tarih == value)
                return;

            Model.Tarih = value;
            OnPropertyChanged();
        }
    }
}