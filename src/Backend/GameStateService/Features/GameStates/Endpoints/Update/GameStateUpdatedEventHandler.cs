using FastEndpoints;
using GameStateService.Features.GameStates.Entities;

namespace GameStateService.Features.GameStates.Endpoints.Update;

public sealed record GameStateUpdatedEvent : IEvent
{
    public required GameEntity GameState { get; init; }

    public sealed class GameStateUpdatedEventHandler(IGameStateUpdatedEventPublisher eventPublisher) : IEventHandler<GameStateUpdatedEvent>
    {
        public async Task HandleAsync(GameStateUpdatedEvent eventModel, CancellationToken ct)
        {
            await eventPublisher.PublishAsync(eventModel, ct);
        }
    }
}
