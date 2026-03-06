using GameService.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit;

namespace GameService.Tests;

public sealed class PostgresTestContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder().Build();

    public string ConnectionString => _postgres.GetConnectionString();

    public async Task InitializeAsync()
    {
        try
        {
            await _postgres.StartAsync();
        }
        catch
        {
            await _postgres.DisposeAsync();
            throw;
        }
    }

    public async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
    }

    public ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:postgres"] = ConnectionString
            })
            .Build();

        services.AddGamePersistence(config);
        return services.BuildServiceProvider();
    }

    public static async Task ResetDatabaseAsync(IServiceProvider provider)
    {
        using var scope = provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GamePersistenceDbContext>();
        await db.Database.ExecuteSqlRawAsync("DROP TABLE IF EXISTS game_model;");
        await db.Database.ExecuteSqlRawAsync("DROP TABLE IF EXISTS player_model;");
        await db.Database.ExecuteSqlRawAsync("CREATE TABLE player_model (id varchar(36) PRIMARY KEY, name varchar(50) NOT NULL);");
        await db.Database.ExecuteSqlRawAsync("CREATE TABLE game_model (id uuid PRIMARY KEY, status varchar(20) NOT NULL, created_at_utc timestamptz NOT NULL, updated_at_utc timestamptz NULL, player1_id varchar(36) NOT NULL REFERENCES player_model(id), player2_id varchar(36) NULL REFERENCES player_model(id));");
        await db.Database.ExecuteSqlRawAsync("CREATE INDEX ix_game_model_status ON game_model(status);");
    }
}

[CollectionDefinition(Name)]
public sealed class PostgresCollection : ICollectionFixture<PostgresTestContainerFixture>
{
    public const string Name = "postgres-game-service";
}
