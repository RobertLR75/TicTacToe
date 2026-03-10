using FastEndpoints;

namespace GameService.Endpoints.Games.UpdateStatus;

public sealed record GameStatusUpdatedEvent : IEvent
{
    public Guid GameId { get; init; }
    public required string NewStatus { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
    
    public class GameStatusUpdatedEventHandler : IEventHandler<GameStatusUpdatedEvent>
    {
        public async Task HandleAsync(GameStatusUpdatedEvent eventModel, CancellationToken ct)
        {
            // Placeholder for future logic
            await Task.CompletedTask;
        }
    }
}

