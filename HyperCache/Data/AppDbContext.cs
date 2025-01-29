using HyperCache.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HyperCache.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<CustomProperty> CustomProperties { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var customProperty = modelBuilder.Entity<CustomProperty>();
        customProperty.HasKey(cp => cp.Id);

        // Configures RowVersion for optimistic concurrency control.
        customProperty
            .Property(cp => cp.RowVersion)
            .IsRowVersion()
            .HasConversion<byte[]>();

        // Configure required properties for the CustomProperty entity.
        customProperty.Property(cp => cp.Name).IsRequired();
        customProperty.Property(cp => cp.Value).IsRequired();
        customProperty.Property(cp => cp.CreatedBy).IsRequired();
        customProperty.Property(cp => cp.ModifiedBy).IsRequired();
        customProperty.Property(cp => cp.ParentTable).IsRequired();
    }
}