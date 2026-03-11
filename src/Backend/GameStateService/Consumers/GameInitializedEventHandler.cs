using FastEndpoints;
using GameStateService.Services;

namespace GameStateService.Consumers;

public sealed record GameInitializedEvent : IEvent
{
    public required Models.GameState GameState { get; init; }

    public sealed class GameInitializedEventHandler(IGameEventPublisher eventPublisher) : IEventHandler<GameInitializedEvent>
    {
        public async Task HandleAsync(GameInitializedEvent eventModel, CancellationToken ct)
        {
            await eventPublisher.PublishEventAsync(GameStateEventMapper.ToGameStateInitialized(eventModel.GameState), ct);
        }
    }
}
