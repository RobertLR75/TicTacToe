using GameService.Models;
using GameService.Persistence;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.PostgreSql.EntityFramework;
using TicTacToe.Testing;

namespace GameService.IntegrationTests;

public abstract class GameServiceIntegrationTestBase
{
    protected GameServiceIntegrationTestBase(PostgresTestContainerFixture fixture)
    {
        Fixture = fixture;
    }

    protected PostgresTestContainerFixture Fixture { get; }

    protected Player CreatePlayer(string id = "p1", string name = "Alice")
        => new()
        {
            Id = id,
            Name = name
        };

    protected Game CreateGame(
        GameStatus status = GameStatus.Created,
        Guid? id = null,
        Player? player1 = null,
        Player? player2 = null,
        DateTimeOffset? createdAt = null,
        DateTimeOffset? updatedAt = null)
        => new()
        {
            Id = id ?? Guid.NewGuid(),
            Status = status,
            CreatedAt = createdAt ?? DateTimeOffset.UtcNow,
            UpdatedAt = updatedAt,
            Player1 = player1 ?? CreatePlayer(),
            Player2 = player2
        };

    protected GameServiceWebApplicationFactory CreateFactory()
        => new(Fixture.ConnectionString);

    protected ServiceProvider CreateServiceProvider(Action<IServiceCollection>? configureServices = null)
    {
        var services = new ServiceCollection();
        var config = TestConfigurationFactory.Build(new Dictionary<string, string?>
        {
            ["ConnectionStrings:postgres"] = Fixture.ConnectionString
        });

        services.AddGamePersistence(config);
        configureServices?.Invoke(services);

        return services.BuildServiceProvider();
    }

    protected static Task ResetDatabaseAsync(IServiceProvider provider)
        => PostgresTestContainerFixture.ResetDatabaseAsync(provider);

    protected static async Task<Game> PersistGameAsync(IServiceProvider services, Game game)
    {
        using var scope = services.CreateScope();
        var store = scope.ServiceProvider.GetRequiredService<IPostgresSqlStorageService<Game>>();
        await store.CreateAsync(game);
        return game;
    }

    protected static async Task<Game> SeedGameAsync(IServiceProvider services, GameStatus status, string playerId = "p1", string playerName = "Alice")
    {
        using var scope = services.CreateScope();
        var store = scope.ServiceProvider.GetRequiredService<IPostgresSqlStorageService<Game>>();

        var game = new Game
        {
            Id = Guid.NewGuid(),
            Status = status,
            Player1 = new Player
            {
                Id = playerId,
                Name = playerName
            },
            UpdatedAt = status == GameStatus.Created ? null : DateTimeOffset.UtcNow
        };

        await store.CreateAsync(game);
        return game;
    }

    protected static async Task<Game?> GetGameAsync(IServiceProvider services, Guid id)
    {
        using var scope = services.CreateScope();
        var store = scope.ServiceProvider.GetRequiredService<IPostgresSqlStorageService<Game>>();
        return await store.GetAsync(id);
    }
}
