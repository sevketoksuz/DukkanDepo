using Microsoft.EntityFrameworkCore.Design;

namespace DukkanDepo.Infrastructure.Persistence.Context;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        return new AppDbContext();
    }
}