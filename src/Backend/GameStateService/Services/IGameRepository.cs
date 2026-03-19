using GameStateService.Features.GameStates.Entities;

namespace GameStateService.Services;

public interface IGameRepository
{
    Task<GameEntity> CreateGameAsync(string? gameId = null, CancellationToken ct = default);

    Task<GameEntity?> GetGameAsync(string gameId, CancellationToken ct = default);

    Task UpdateGameAsync(GameEntity game, CancellationToken ct = default);

    Task DeleteGameAsync(string gameId, CancellationToken ct = default);
}
