using FastEndpoints;
using GameStateService.Services;

namespace GameStateService.Endpoints.Games.Create;

public sealed record GameCreatedEvent : IEvent
{
    public required Models.GameState Game { get; init; }
    
    
    public sealed class GameCreatedEventHandler(IGameEventPublisher eventPublisher) : IEventHandler<GameCreatedEvent>
    {
        public async Task HandleAsync(GameCreatedEvent eventModel, CancellationToken ct)
        {
            await eventPublisher.PublishEventAsync(GameEventMapper.ToGameStateInitializedEvent(eventModel.Game), ct);
        }
    }
}

