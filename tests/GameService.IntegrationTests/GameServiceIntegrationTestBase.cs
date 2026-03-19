using GameService.Features.Games.Entities;
using GameService.Persistence;
using GameService.Services;
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

    protected PlayerEntity CreatePlayer(string id = "11111111-1111-1111-1111-111111111111", string name = "Alice")
        => new()
        {
            Id = Guid.Parse(id),
            Name = name
        };

    protected GameEntity CreateGame(
        GameStatus status = GameStatus.Created,
        Guid? id = null,
        PlayerEntity? player1 = null,
        PlayerEntity? player2 = null,
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

    protected static async Task<GameEntity> PersistGameAsync(IServiceProvider services, GameEntity gameEntity)
    {
        using var scope = services.CreateScope();
        var store = scope.ServiceProvider.GetRequiredService<IGameStorageService>();
        await store.CreateAsync(gameEntity);
        return gameEntity;
    }

    protected static async Task<GameEntity> SeedGameAsync(IServiceProvider services, GameStatus status, string playerId = "11111111-1111-1111-1111-111111111111", string playerName = "Alice")
    {
        using var scope = services.CreateScope();
        var store = scope.ServiceProvider.GetRequiredService<IGameStorageService>();

        var game = new GameEntity
        {
            Id = Guid.NewGuid(),
            Status = status,
            Player1 = new PlayerEntity
            {
                Id = Guid.Parse(playerId),
                Name = playerName
            },
            UpdatedAt = status == GameStatus.Created ? null : DateTimeOffset.UtcNow
        };

        await store.CreateAsync(game);
        return game;
    }

    protected static async Task<GameEntity?> GetGameAsync(IServiceProvider services, Guid id)
    {
        using var scope = services.CreateScope();
        var store = scope.ServiceProvider.GetRequiredService<IGameStorageService>();
        return await store.GetAsync(id);
    }
}
