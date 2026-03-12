using GameService.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Testcontainers.PostgreSql;
using TicTacToe.Testing;
using Xunit;

namespace GameService.IntegrationTests;

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
        var config = TestConfigurationFactory.Build(new Dictionary<string, string?>
        {
            ["ConnectionStrings:postgres"] = ConnectionString
        });

        services.AddGamePersistence(config);

        return services.BuildServiceProvider();
    }

    public static async Task ResetDatabaseAsync(IServiceProvider provider)
    {
        using var scope = provider.CreateScope();
        var connectionString = scope.ServiceProvider.GetRequiredService<DbContext>().Database.GetDbConnection().ConnectionString;

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("A PostgreSQL connection string is required to reset the GameService test database.");
        }

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var resetStatements = new[]
        {
            "DROP SCHEMA IF EXISTS \"public\" CASCADE",
            "CREATE SCHEMA \"public\""
        };

        await using var command = connection.CreateCommand();
        command.CommandText = string.Join("; ", resetStatements) + ";";
        await command.ExecuteNonQueryAsync();

        await WaitForPersistenceReadyAsync(provider);
    }

    private static async Task WaitForPersistenceReadyAsync(IServiceProvider provider)
    {
        using var scope = provider.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<IGamePersistenceInitializer>();
        var readinessState = scope.ServiceProvider.GetRequiredService<GamePersistenceReadinessState>();

        for (var attempt = 0; attempt < 10; attempt++)
        {
            if (await initializer.EnsureInitializedAsync())
            {
                return;
            }

            await Task.Delay(200);
        }

        throw new InvalidOperationException(
            $"Game persistence initializer could not prepare the integration test database. Last error: {readinessState.LastErrorMessage ?? "unknown"}");
    }
}

[CollectionDefinition(Name)]
public sealed class PostgresCollection : ICollectionFixture<PostgresTestContainerFixture>
{
    public const string Name = "postgres-game-service";
}
