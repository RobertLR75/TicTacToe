using FastEndpoints;
using Service.Contracts.CreateGame;

namespace GameService.Endpoints.Games.Create;

public record GameCreatedEvent : GameCreated, IEvent
{

}

public class GameCreatedEventHandler : IEventHandler<GameCreatedEvent>
{
    public async Task HandleAsync(GameCreatedEvent model, CancellationToken ct)
    {
        // Placeholder for future logic (e.g. notifications, analytics)
        await Task.CompletedTask;
    }
}

