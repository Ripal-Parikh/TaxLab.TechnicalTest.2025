using Microsoft.EntityFrameworkCore;
using TaxLab.Data.Entities;

namespace TaxLab.Data;

public class TaxLabDbContext : DbContext
{
    public TaxLabDbContext(DbContextOptions<TaxLabDbContext> options) : base(options) { }

    public DbSet<TaxRateEntry> TaxRates => Set<TaxRateEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaxRateEntry>(entity =>
        {
            entity.HasKey(e => e.TaxRateId);

            entity.Property(e => e.TaxRateId)
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.BandStart)
                  .HasColumnType("TEXT")
                  .IsRequired();

            entity.Property(e => e.BandFinish)
                  .HasColumnType("TEXT");

            entity.Property(e => e.TaxRate)
                  .HasColumnType("TEXT")
                  .IsRequired();

            entity.Property(e => e.CreatedDateUTC)
                  .IsRequired();
        });
    }
}
