using FastEndpoints;
using GameStateService.Features.GameStates.Entities;
using GameStateService.Services;

namespace GameStateService.Consumers;

public sealed record GameInitializedEvent : IEvent
{
    public required GameEntity GameState { get; init; }

    public sealed class GameInitializedEventHandler(IGameInitializedPublisher publisher) : IEventHandler<GameInitializedEvent>
    {
        public async Task HandleAsync(GameInitializedEvent eventModel, CancellationToken ct)
        {
            await publisher.PublishAsync(eventModel, ct);
        }
    }
}
