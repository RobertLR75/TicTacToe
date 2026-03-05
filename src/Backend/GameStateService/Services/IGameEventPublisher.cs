using GameStateService.Models;

namespace GameStateService.Services;

public interface IGameEventPublisher
{
    Task PublishGameCreatedAsync(GameState game, CancellationToken ct = default);

    Task PublishGameStateUpdatedAsync(GameState game, CancellationToken ct = default);
}
