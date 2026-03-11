using FastEndpoints;
using GameStateService.Services;

namespace GameStateService.Endpoints.Games.MakeMove;

public sealed record GameStateUpdatedEvent : IEvent
{
    public required Models.GameState GameState { get; init; }

    public sealed class GameStateUpdatedEventHandler(IGameEventPublisher eventPublisher) : IEventHandler<GameStateUpdatedEvent>
    {
        public async Task HandleAsync(GameStateUpdatedEvent eventModel, CancellationToken ct)
        {
            await eventPublisher.PublishEventAsync(GameStateEventMapper.ToGameStateUpdated(eventModel.GameState), ct);
        }
    }
}
