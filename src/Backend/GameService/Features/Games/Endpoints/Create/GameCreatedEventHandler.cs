using FastEndpoints;
using GameService.Features.Games.Entities;
using GameService.Services;

namespace GameService.Features.Games.Endpoints.Create;

public record GameCreatedEvent : IEvent
{

    public required GameEntity GameEntity { get; init; }
    public class GameCreatedEventHandler(ICreateGameEventPublisher eventPublisher) : IEventHandler<GameCreatedEvent>
    {
        public async Task HandleAsync(GameCreatedEvent model, CancellationToken ct)
        {
            await eventPublisher.PublishAsync(model, ct);
        }
    }
}



