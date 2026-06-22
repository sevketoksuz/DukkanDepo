using System.Linq.Expressions;
using DukkanDepo.Application.Abstractions.Repositories;
using DukkanDepo.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DukkanDepo.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> DbSet;

    public Repository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public IQueryable<T> AsQueryable()
    {
        return DbSet.AsQueryable();
    }

    public virtual Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return DbSet.ToListAsync(cancellationToken);
    }

    public virtual ValueTask<T?> GetByIdValueTaskAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return DbSet.FindAsync([id], cancellationToken);
    }

    public virtual async Task<T?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    public virtual Task<List<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return DbSet
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public virtual Task AddAsync(
        T entity,
        CancellationToken cancellationToken = default)
    {
        DbSet.Add(entity);
        return Task.CompletedTask;
    }

    public virtual Task UpdateAsync(
        T entity,
        CancellationToken cancellationToken = default)
    {
        DbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(
        T entity,
        CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Context.SaveChangesAsync(cancellationToken);
    }
}