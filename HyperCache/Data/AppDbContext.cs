using HyperCache.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HyperCache.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<CustomProperty> CustomProperties { get; set; }
}