using DukkanDepo.Domain.Entities;
using DukkanDepo.Infrastructure.Persistence.Context;

namespace DukkanDepo.Infrastructure.Persistence.Repositories;

/// <summary>
/// Eski kullanımlar için tip argümansız köprü repository.
/// Ana kullanım: UrunRepository&lt;YazlikUrun&gt; veya UrunRepository&lt;KislikUrun&gt;.
/// </summary>
public sealed class UrunRepository : UrunRepository<Urun>
{
    public UrunRepository(AppDbContext context)
        : base(context)
    {
    }
}