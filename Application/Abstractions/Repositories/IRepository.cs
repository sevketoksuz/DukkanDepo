using System.Linq.Expressions;

namespace DukkanDepo.Application.Abstractions.Repositories;

public interface IRepository<T> where T : class
{
    IQueryable<T> AsQueryable();

    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<T?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<List<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        T entity,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        T entity,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        T entity,
        CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}