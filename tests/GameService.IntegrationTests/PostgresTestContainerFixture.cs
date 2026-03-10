using GameService.Persistence;
using Microsoft.Extensions.DependencyInjection;
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

        var provider = services.BuildServiceProvider();
        provider.ApplyGameMigrations();
        return provider;
    }

    public static Task ResetDatabaseAsync(IServiceProvider provider)
    {
        return DatabaseSchemaReset.ResetAsync(provider);
    }
}

[CollectionDefinition(Name)]
public sealed class PostgresCollection : ICollectionFixture<PostgresTestContainerFixture>
{
    public const string Name = "postgres-game-service";
}
