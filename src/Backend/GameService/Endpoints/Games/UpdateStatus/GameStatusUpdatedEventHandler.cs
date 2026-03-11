using FastEndpoints;
using GameService.Services;

namespace GameService.Endpoints.Games.UpdateStatus;

public sealed record GameStatusUpdatedEvent : IEvent
{
    public required Models.Game Game { get; init; }
    
    public class GameStatusUpdatedEventHandler(IGameEventPublisher eventPublisher) : IEventHandler<GameStatusUpdatedEvent>
    {
        public async Task HandleAsync(GameStatusUpdatedEvent eventModel, CancellationToken ct)
        {
            // Placeholder for future logic
            await eventPublisher.PublishEventAsync(GameEventMapper.ToGameStateUpdated(eventModel.Game), ct);
        }
    }
}


