using FastEndpoints;
using GameService.Services;

namespace GameService.Endpoints.Games.Create;

public record GameCreatedEvent : IEvent
{

    public required Models.Game Game { get; init; }
    public class GameCreatedEventHandler(IGameEventPublisher eventPublisher) : IEventHandler<GameCreatedEvent>
    {
        public async Task HandleAsync(GameCreatedEvent model, CancellationToken ct)
        {
            await eventPublisher.PublishEventAsync(GameEventMapper.ToGameCreated(model.Game), ct);
        }
    }
}



