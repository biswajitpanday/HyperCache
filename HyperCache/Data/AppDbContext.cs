using HyperCache.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HyperCache.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<CustomProperty> CustomProperties { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CustomProperty>().HasData(
            new CustomProperty { Id = Guid.CreateVersion7() },
            new CustomProperty { Id = Guid.CreateVersion7() }
            );
    }
}