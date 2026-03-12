using FluentMigrator.Runner;
using GameService.Models;
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
    public void AddGamePersistence_registers_game_db_context_and_db_context_alias()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:postgres"] = "Host=localhost;Database=test;Username=test;Password=test"
            })
            .Build();

        services.AddGamePersistence(configuration);
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var genericDbContext = scope.ServiceProvider.GetRequiredService<GenericDbContext<Game>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        var readinessState = scope.ServiceProvider.GetRequiredService<GamePersistenceReadinessState>();
        var initializer = scope.ServiceProvider.GetRequiredService<IGamePersistenceInitializer>();

        Assert.Same(genericDbContext, dbContext);
        Assert.NotNull(runner);
        Assert.NotNull(readinessState);
        Assert.NotNull(initializer);
    }

    [Fact]
    public void AddGamePersistence_uses_postgres_db_fallback_when_primary_connection_string_is_missing()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:postgres-db"] = "Host=localhost;Database=fallback;Username=test;Password=test"
            })
            .Build();

        services.AddGamePersistence(configuration);
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var genericDbContext = scope.ServiceProvider.GetRequiredService<GenericDbContext<Game>>();

        Assert.NotNull(genericDbContext);
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
