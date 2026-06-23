using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using DukkanDepo.Domain.Entities;
using DukkanDepo.Infrastructure.Persistence.Repositories;
using DukkanDepo.Presentation.Commands;
using DukkanDepo.Presentation.Common;

namespace DukkanDepo.Presentation.ViewModels.Main;

public class MainViewModel<TProduct> : ObservableObject
    where TProduct : Urun, new()
{
    private readonly UrunRepository<TProduct> _repository;
    private readonly UrunFilterState _filters = new();

    private readonly DispatcherTimer _filterTimer = new()
    {
        Interval = TimeSpan.FromMilliseconds(250)
    };

    private bool _suppressAutoFilter;
    private int _sayfa = 1;
    private int _sayfaBoyutu = 20;
    private int _toplamSayfa;

    private string _lastSortColumn = "Id";
    private ListSortDirection _lastSortDirection = ListSortDirection.Descending;

    private TProduct? _seciliUrun;

    public MainViewModel(UrunRepository<TProduct> repository)
    {
        _repository = repository;

        UrunlerView = CollectionViewSource.GetDefaultView(Urunler);

        _filterTimer.Tick += (_, _) =>
        {
            _filterTimer.Stop();
            _sayfa = 1;
            VerileriYukle(_lastSortColumn, _lastSortDirection);
        };

        OncekiCommand = new RelayCommand(_ =>
        {
            if (_sayfa <= 1)
                return;

            _sayfa--;
            VerileriYukle(_lastSortColumn, _lastSortDirection);
        });

        SonrakiCommand = new RelayCommand(_ =>
        {
            if (_sayfa >= _toplamSayfa)
                return;

            _sayfa++;
            VerileriYukle(_lastSortColumn, _lastSortDirection);
        });

        SilCommand = new RelayCommand(
            _ => UrunSil(),
            _ => SeciliUrun is not null);

        TemizleFiltreCommand = new RelayCommand(_ =>
        {
            _suppressAutoFilter = true;

            _filters.Clear();
            RaiseAllFilterProperties();

            _suppressAutoFilter = false;

            _sayfa = 1;
            VerileriYukle(_lastSortColumn, _lastSortDirection);
        });

        YenileCommand = new RelayCommand(_ =>
            VerileriYukle(_lastSortColumn, _lastSortDirection));

        VerileriYukle();
    }

    public ObservableCollection<TProduct> Urunler { get; } = new();

    public ICollectionView UrunlerView { get; private set; }

    public Action? BeforeReload { get; set; }

    public TProduct? SeciliUrun
    {
        get => _seciliUrun;
        set
        {
            if (value is null || value.GetType().Name == "NamedObject")
                return;

            if (EqualityComparer<TProduct?>.Default.Equals(_seciliUrun, value))
                return;

            _seciliUrun = value;
            OnPropertyChanged();
            RelayCommand.RaiseCanExecuteChanged();
        }
    }

    public string? FiltreKod
    {
        get => _filters.Kod;
        set
        {
            if (_filters.Kod == value)
                return;

            _filters.Kod = value;
            OnPropertyChanged();
            OnFilterChanged();
        }
    }

    public string? FiltreModel
    {
        get => _filters.Model;
        set
        {
            if (_filters.Model == value)
                return;

            _filters.Model = value;
            OnPropertyChanged();
            OnFilterChanged();
        }
    }

    public string? FiltreCins
    {
        get => _filters.Cins;
        set
        {
            if (_filters.Cins == value)
                return;

            _filters.Cins = value;
            OnPropertyChanged();
            OnFilterChanged();
        }
    }

    public string? FiltreAdet
    {
        get => _filters.Adet;
        set
        {
            if (_filters.Adet == value)
                return;

            _filters.Adet = value;
            OnPropertyChanged();
            OnFilterChanged();
        }
    }

    public string? FiltreIskontoMin
    {
        get => _filters.IskontoMin;
        set
        {
            if (_filters.IskontoMin == value)
                return;

            _filters.IskontoMin = value;
            OnPropertyChanged();
            OnFilterChanged();
        }
    }

    public string? FiltreIskontoMax
    {
        get => _filters.IskontoMax;
        set
        {
            if (_filters.IskontoMax == value)
                return;

            _filters.IskontoMax = value;
            OnPropertyChanged();
            OnFilterChanged();
        }
    }

    public string? FiltreSatisMin
    {
        get => _filters.SatisMin;
        set
        {
            if (_filters.SatisMin == value)
                return;

            _filters.SatisMin = value;
            OnPropertyChanged();
            OnFilterChanged();
        }
    }

    public string? FiltreSatisMax
    {
        get => _filters.SatisMax;
        set
        {
            if (_filters.SatisMax == value)
                return;

            _filters.SatisMax = value;
            OnPropertyChanged();
            OnFilterChanged();
        }
    }

    public DateTime? FiltreTarihBaslangic
    {
        get => _filters.TarihBaslangic;
        set
        {
            if (_filters.TarihBaslangic == value)
                return;

            _filters.TarihBaslangic = value;
            OnPropertyChanged();
            OnFilterChanged();
        }
    }

    public DateTime? FiltreTarihBitis
    {
        get => _filters.TarihBitis;
        set
        {
            if (_filters.TarihBitis == value)
                return;

            _filters.TarihBitis = value;
            OnPropertyChanged();
            OnFilterChanged();
        }
    }

    public int SayfaBoyutu
    {
        get => _sayfaBoyutu;
        set
        {
            if (_sayfaBoyutu == value)
                return;

            _sayfaBoyutu = value;
            _sayfa = 1;

            OnPropertyChanged();
            OnPropertyChanged(nameof(SayfaMetni));

            VerileriYukle(_lastSortColumn, _lastSortDirection);
        }
    }

    public int[] SayfaBoyutuSecenekleri { get; } =
    [
        20,
        40,
        60,
        80,
        100
    ];

    public string SayfaMetni => $"{_sayfa} / {Math.Max(_toplamSayfa, 1)}";

    protected void EnsureDraftRow()
    {
        if (Urunler.Any(product => product.Id == 0))
            return;

        Urunler.Add(new TProduct
        {
            Tarih = DateTime.Now
        });
    }

    protected static bool CanPersistProduct(TProduct? product)
    {
        if (product is null)
            return false;

        return !string.IsNullOrWhiteSpace(product.Kod);
    }

    public ICommand YenileCommand { get; }

    public ICommand TemizleFiltreCommand { get; }

    public ICommand OncekiCommand { get; }

    public ICommand SonrakiCommand { get; }

    public ICommand SilCommand { get; }

    public void VerileriYukle(
        string? sortColumn = null,
        ListSortDirection? sortDirection = null)
    {
        BeforeReload?.Invoke();

        if (!string.IsNullOrWhiteSpace(sortColumn))
            _lastSortColumn = sortColumn;

        if (sortDirection.HasValue)
            _lastSortDirection = sortDirection.Value;

        var culture = CultureInfo.CurrentCulture;

        var query = _repository.Query();
        query = UrunQueryHelper.ApplyDbFilters(query, _filters, culture);

        var list = query.ToList();

        list = UrunInMemoryHelper.ApplyTextFilters(list, _filters);
        list = UrunInMemoryHelper.ApplySort(list, _lastSortColumn, _lastSortDirection);

        var (pageItems, totalPages) = PagingHelper.Paginate(
            list,
            _sayfa,
            SayfaBoyutu);

        _toplamSayfa = totalPages;

        Urunler.Clear();

        foreach (var product in pageItems)
            Urunler.Add(product);

        EnsureDraftRow();

        OnPropertyChanged(nameof(SayfaMetni));
        UrunlerView.Refresh();
    }

    private void UrunSil()
    {
        if (SeciliUrun is null || SeciliUrun.Id == 0)
            return;

        _repository
            .DeleteByIdAsync(SeciliUrun.Id)
            .GetAwaiter()
            .GetResult();

        SeciliUrun = null;

        VerileriYukle(_lastSortColumn, _lastSortDirection);
    }

    private void OnFilterChanged()
    {
        if (_suppressAutoFilter)
            return;

        _filterTimer.Stop();
        _filterTimer.Start();
    }

    private void RaiseAllFilterProperties()
    {
        OnPropertyChanged(nameof(FiltreKod));
        OnPropertyChanged(nameof(FiltreModel));
        OnPropertyChanged(nameof(FiltreCins));
        OnPropertyChanged(nameof(FiltreAdet));
        OnPropertyChanged(nameof(FiltreIskontoMin));
        OnPropertyChanged(nameof(FiltreIskontoMax));
        OnPropertyChanged(nameof(FiltreSatisMin));
        OnPropertyChanged(nameof(FiltreSatisMax));
        OnPropertyChanged(nameof(FiltreTarihBaslangic));
        OnPropertyChanged(nameof(FiltreTarihBitis));
    }
}