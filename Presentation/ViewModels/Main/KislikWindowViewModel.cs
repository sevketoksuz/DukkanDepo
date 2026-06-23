using DukkanDepo.Domain.Entities;
using DukkanDepo.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DukkanDepo.Presentation.ViewModels.Main;

public sealed class KislikWindowViewModel : MainViewModel<KislikUrun>
{
    private readonly UrunRepository<KislikUrun> _repository;

    public KislikWindowViewModel(UrunRepository<KislikUrun> repository)
        : base(repository)
    {
        _repository = repository;
    }

    public IQueryable<Urun> UrunQuery()
    {
        return _repository
            .Query()
            .Cast<Urun>();
    }

    public List<Urun> GetAllUrunNoTracking()
    {
        return _repository
            .Query()
            .AsNoTracking()
            .Cast<Urun>()
            .ToList();
    }

    public Task DeleteManyByIdsAsync(
        IEnumerable<int> ids,
        CancellationToken cancellationToken = default)
    {
        return _repository.DeleteManyByIdsAsync(
            ids,
            cancellationToken);
    }

    public void SaveRow(KislikUrun? product)
    {
        if (product is null)
            return;

        if (!CanPersistProduct(product))
        {
            EnsureDraftRow();
            return;
        }

        product.Recalculate();

        var wasNew = product.Id == 0;

        if (wasNew)
        {
            _repository
                .AddAsync(product)
                .GetAwaiter()
                .GetResult();
        }
        else
        {
            _repository
                .UpdateAsync(product)
                .GetAwaiter()
                .GetResult();
        }

        _repository
            .SaveChangesAsync()
            .GetAwaiter()
            .GetResult();

        if (wasNew)
            EnsureDraftRow();
    }
}