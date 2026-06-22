using DukkanDepo.Domain.Entities;
using DukkanDepo.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DukkanDepo.Infrastructure.Persistence.Repositories;

public class UrunRepository<TProduct> : Repository<TProduct>
    where TProduct : Urun
{
    public UrunRepository(AppDbContext context)
        : base(context)
    {
    }

    public IQueryable<TProduct> Query()
    {
        return Context.Set<TProduct>().AsQueryable();
    }

    public Task<int> SumAdetAsync(CancellationToken cancellationToken = default)
    {
        return Context
            .Set<TProduct>()
            .SumAsync(product => product.Adet ?? 0, cancellationToken);
    }

    public Task<decimal> SumTutarAsync(CancellationToken cancellationToken = default)
    {
        return Context
            .Set<TProduct>()
            .SumAsync(product => product.Tutar ?? 0m, cancellationToken);
    }

    public async Task DeleteByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entity = await Context
            .Set<TProduct>()
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);

        if (entity is null)
            return;

        Context.Remove(entity);

        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteManyByIdsAsync(
        IEnumerable<int> ids,
        CancellationToken cancellationToken = default)
    {
        var idList = ids
            .Distinct()
            .ToList();

        if (idList.Count == 0)
            return;

        var productsToRemove = await Context
            .Set<TProduct>()
            .Where(product => idList.Contains(product.Id))
            .ToListAsync(cancellationToken);

        if (productsToRemove.Count == 0)
            return;

        Context.RemoveRange(productsToRemove);

        await Context.SaveChangesAsync(cancellationToken);
    }
}