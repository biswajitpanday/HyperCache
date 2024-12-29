using HyperCache.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HyperCache.Api.Data;

public class DataSeeder(AppDbContext context)
{
    public async Task SeedCustomPropertiesAsync()
    {
        if (await context.CustomProperties.AnyAsync())
            return; // Skip seeding if data already exists.

        var customProperties = GenerateCustomProperties(1_000_000);
        foreach (var batch in customProperties.Chunk(10_000))
        {
            await context.CustomProperties.AddRangeAsync(batch);
            await context.SaveChangesAsync();
        }
    }

    private List<CustomProperty> GenerateCustomProperties(int count)
    {
        var random = new Random();
        var customProperties = new List<CustomProperty>();
        var parentTableNames = new[] { "Orders", "Products", "Users", "Invoices" };
        var users = new[] { "Alice", "Bob", "Charlie", "Diana" };

        for (int i = 0; i < count; i++)
        {
            var parentId = Guid.NewGuid();
            var parentTable = parentTableNames[random.Next(parentTableNames.Length)];
            customProperties.Add(new CustomProperty
            {
                Id = Guid.NewGuid(),
                ParentId = parentId,
                Name = $"{parentTable}_Property_{i}",
                Value = $"Value_{random.Next(1, 1000)}",
                CreatedOn = DateTime.UtcNow.AddDays(-random.Next(1, 365)),
                ModifiedOn = DateTime.UtcNow.AddMinutes(-random.Next(1, 10000)),
                CreatedBy = users[random.Next(users.Length)],
                ModifiedBy = users[random.Next(users.Length)],
                ParentTable = parentTable
            });
        }

        return customProperties;
    }
}