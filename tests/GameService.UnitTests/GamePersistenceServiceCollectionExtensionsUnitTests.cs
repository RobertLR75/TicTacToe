using FluentMigrator.Runner;
using GameService.Features.Games.Entities;
using GameService.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using SharedLibrary.PostgreSql.EntityFramework;
using Xunit;

namespace GameService.UnitTests;

public class GamePersistenceServiceCollectionExtensionsUnitTests
{
    [Fact]
    public void AddGamePersistence_throws_when_supported_postgres_configuration_is_missing()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        var ex = Assert.Throws<InvalidOperationException>(() => services.AddGamePersistence(configuration));

        Assert.Contains("PostgreSQL configuration is required", ex.Message);
        Assert.Contains("ConnectionStrings:postgres", ex.Message);
        Assert.Contains("ConnectionStrings:postgres-db", ex.Message);
    }

    [Fact]
    public void ResolveRequired_prefers_aspire_postgres_db_connection_string_when_multiple_sources_exist()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:postgres"] = "Host=localhost;Database=primary;Username=test;Password=test",
                ["ConnectionStrings:postgres-db"] = "Host=localhost;Database=aspire;Username=test;Password=test"
            })
            .Build();

        var resolved = PostgresConnectionStringResolver.ResolveRequired(configuration, "GameService");

        Assert.Contains("Database=aspire", resolved);
    }

    [Fact]
    public void ApplyGameMigrations_invokes_migration_runner()
    {
        var services = new ServiceCollection();
        var runner = Substitute.For<IMigrationRunner>();
        services.AddScoped(_ => runner);
        using var provider = services.BuildServiceProvider();

        provider.ApplyGameMigrations();

        runner.Received(1).MigrateUp();
    }
}
