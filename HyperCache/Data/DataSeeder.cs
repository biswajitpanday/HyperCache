using HyperCache.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HyperCache.Api.Data;

public class DataSeeder(AppDbContext context, IOptions<SeedOptions> options)
{
    // Sample parent table names and user names, picked with Random.GetItems (arrays convert to ReadOnlySpan<T> implicitly).
    private static readonly string[] ParentTableNames = ["Orders", "Products", "Users", "Invoices"];
    private static readonly string[] Users = ["Alice", "Bob", "Charlie", "Diana"];

    /// <summary>
    /// Seeds the CustomProperties table with sample data if it is empty.
    /// The row count comes from <see cref="SeedOptions.Count"/> (default 1,000,000).
    /// </summary>
    public async Task SeedCustomPropertiesAsync(CancellationToken cancellationToken = default)
    {
        if (await context.CustomProperties.AnyAsync(cancellationToken))
            return; // Skip seeding if data already exists.

        // Rows are generated lazily and inserted in batches so the full set is never held in memory.
        foreach (var batch in GenerateCustomProperties(options.Value.Count).Chunk(10_000))
        {
            await context.CustomProperties.AddRangeAsync(batch, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            context.ChangeTracker.Clear();
        }
    }

    /// <summary>
    /// Generates custom properties with random data, one at a time.
    /// </summary>
    /// <param name="count">The number of custom properties to generate.</param>
    private static IEnumerable<CustomProperty> GenerateCustomProperties(int count)
    {
        var random = Random.Shared;

        for (var i = 0; i < count; i++)
        {
            var parentTable = random.GetItems(ParentTableNames, 1)[0];
            yield return new CustomProperty
            {
                Id = Guid.NewGuid(),
                ParentId = Guid.NewGuid(),
                Name = $"{parentTable}_Property_{i}",
                Value = $"Value_{random.Next(1, 1000)}",
                CreatedOn = DateTime.UtcNow.AddDays(-random.Next(1, 365)),
                ModifiedOn = DateTime.UtcNow.AddMinutes(-random.Next(1, 10000)),
                CreatedBy = random.GetItems(Users, 1)[0],
                ModifiedBy = random.GetItems(Users, 1)[0],
                ParentTable = parentTable
            };
        }
    }
}
