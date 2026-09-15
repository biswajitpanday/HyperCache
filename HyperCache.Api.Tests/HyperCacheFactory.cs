using HyperCache.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace HyperCache.Api.Tests;

/// <summary>
/// Boots the real API in-process against a throw-away SQL Server database.
/// Delta reads @@DBTS, so these tests need a reachable SQL Server; there is no in-memory substitute.
///
/// Connection string resolution:
///   1. HYPERCACHE_TEST_CONNECTION environment variable, if set.
///   2. Otherwise the API's own ConnectionStrings:DefaultConnection (appsettings*.json) with the
///      database name replaced by <see cref="TestDatabaseName"/>.
/// The database is dropped and re-created from migrations on every run.
/// </summary>
public sealed class HyperCacheFactory : WebApplicationFactory<Program>
{
    public const string TestDatabaseName = "HyperCacheDB_Test";
    public const string ConnectionEnvVar = "HYPERCACHE_TEST_CONNECTION";
    public const int SeedCount = 200;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(Environments.Development);
        builder.UseSetting("Seed:Count", SeedCount.ToString());

        builder.ConfigureAppConfiguration((context, config) =>
        {
            var connectionString = ResolveConnectionString(context.Configuration);

            // Runs while the host is being built, i.e. before Program's seeder executes.
            RecreateDatabase(connectionString);

            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = connectionString
            });
        });
    }

    private static string ResolveConnectionString(IConfiguration appConfiguration)
    {
        var fromEnv = Environment.GetEnvironmentVariable(ConnectionEnvVar);
        if (!string.IsNullOrWhiteSpace(fromEnv))
            return fromEnv;

        var fromApp = appConfiguration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(fromApp) || fromApp.Contains("<your-server-name>"))
        {
            throw new InvalidOperationException(
                $"No usable SQL Server connection string. Set the {ConnectionEnvVar} environment variable " +
                "or configure ConnectionStrings:DefaultConnection in HyperCache/appsettings.Development.json.");
        }

        return new SqlConnectionStringBuilder(fromApp) { InitialCatalog = TestDatabaseName }.ConnectionString;
    }

    private static void RecreateDatabase(string connectionString)
    {
        // This drops the database. Refuse anything that doesn't look like a test database so a
        // mis-set HYPERCACHE_TEST_CONNECTION can never wipe HyperCacheDB.
        var database = new SqlConnectionStringBuilder(connectionString).InitialCatalog;
        if (!database.EndsWith("_Test", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Refusing to drop database '{database}': test connection strings must target a database whose name ends in '_Test'.");
        }

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        using var db = new AppDbContext(options);
        try
        {
            db.Database.EnsureDeleted();
            db.Database.Migrate();
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException(
                $"Could not reach SQL Server to create the test database '{TestDatabaseName}'. " +
                $"Set {ConnectionEnvVar} to a reachable server. Underlying error: {ex.Message}", ex);
        }
    }
}
