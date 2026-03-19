using FastEndpoints;
using GameService.Features.Games.Entities;
using GameService.Services;

namespace GameService.Features.Games.Endpoints.UpdateStatus;

public sealed record GameStatusUpdatedEvent : IEvent
{
    public required GameEntity GameEntity { get; init; }
    
    public class GameStatusUpdatedEventHandler(IUpdateGameStatusEventPublisher eventPublisher) : IEventHandler<GameStatusUpdatedEvent>
    {
        public async Task HandleAsync(GameStatusUpdatedEvent eventModel, CancellationToken ct)
        {
            // Placeholder for future logic
            await eventPublisher.PublishAsync(eventModel, ct);
        }
    }
}


