using DukkanDepo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.IO;

namespace DukkanDepo.Infrastructure.Persistence.Context;

public class AppDbContext : DbContext
{
    public DbSet<YazlikUrun> YazlikUrunler => Set<YazlikUrun>();
    public DbSet<KislikUrun> KislikUrunler => Set<KislikUrun>();

    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public static string GetDbPath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var directory = Path.Combine(appData, "DukkanDepo");

        Directory.CreateDirectory(directory);

        return Path.Combine(directory, "app.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
            return;

        var dbPath = GetDbPath();

        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Urun>().UseTpcMappingStrategy();

        var decimalToDoubleNullableConverter = new ValueConverter<decimal?, double?>(
            value => value.HasValue ? (double?)value.Value : null,
            value => value.HasValue ? (decimal?)value.Value : null);

        modelBuilder.Entity<Urun>(builder =>
        {
            builder.HasKey(product => product.Id);

            builder.Property(product => product.Kod)
                .HasMaxLength(100);

            builder.Property(product => product.Model)
                .HasMaxLength(200);

            builder.Property(product => product.Cins)
                .HasMaxLength(100);

            builder.Property(product => product.Iskonto)
                .HasConversion(decimalToDoubleNullableConverter);

            builder.Property(product => product.Satis)
                .HasConversion(decimalToDoubleNullableConverter);

            builder.Property(product => product.Tutar)
                .HasConversion(decimalToDoubleNullableConverter);

            builder.Property(product => product.IskontoluTutar)
                .HasConversion(decimalToDoubleNullableConverter);
        });

        modelBuilder.Entity<YazlikUrun>(builder =>
        {
            builder.ToTable("YazlikUrunler");

            builder.Property(product => product.Id)
                .ValueGeneratedOnAdd()
                .HasAnnotation("Sqlite:Autoincrement", true);
        });

        modelBuilder.Entity<KislikUrun>(builder =>
        {
            builder.ToTable("KislikUrunler");

            builder.Property(product => product.Id)
                .ValueGeneratedOnAdd()
                .HasAnnotation("Sqlite:Autoincrement", true);
        });

        base.OnModelCreating(modelBuilder);
    }

    public static void EnsureMigrate()
    {
        using var db = new AppDbContext();

        try
        {
            db.Database.Migrate();
        }
        catch
        {
            // WPF başlangıcında migration hatası uygulamayı direkt düşürmesin diye şimdilik sessiz.
            // UI tarafına geçince bunu kullanıcıya mesaj gösterecek hale getirebiliriz.
        }
    }
}