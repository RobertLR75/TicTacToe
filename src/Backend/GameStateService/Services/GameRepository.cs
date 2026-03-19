using GameStateService.Features.GameStates.Entities;
using SharedLibrary.Interfaces;
using SharedLibrary.Redis;
using Microsoft.Extensions.Caching.Distributed;

namespace GameStateService.Services;

public sealed class GameRepository(IPersistenceService<GameEntity> persistenceService) : IGameRepository
{
    public async Task<GameEntity> CreateGameAsync(string? gameId = null, CancellationToken ct = default)
    {
        var game = new GameEntity();

        if (!string.IsNullOrWhiteSpace(gameId))
        {
            game.GameId = gameId;
        }

        await persistenceService.CreateAsync(game, ct);
        return game;
    }

    public async Task<GameEntity?> GetGameAsync(string gameId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(gameId, out var parsedGameId))
        {
            return null;
        }

        return await persistenceService.GetAsync(parsedGameId, ct);
    }

    public Task UpdateGameAsync(GameEntity game, CancellationToken ct = default)
    {
        game.UpdatedAt = DateTimeOffset.UtcNow;
        return persistenceService.UpdateAsync(game, ct);
    }

    public async Task DeleteGameAsync(string gameId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(gameId, out var parsedGameId))
        {
            return;
        }

        await persistenceService.DeleteAsync(parsedGameId, ct);
    }
}

public sealed class GameRedisPersistenceService(IDistributedCache cache)
    : BaseRedisPersistenceService<GameEntity>(cache)
{
    public override string Name => "game-state-service-games";
}
