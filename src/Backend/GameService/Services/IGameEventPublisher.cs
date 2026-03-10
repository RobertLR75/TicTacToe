using FastEndpoints;
using GameService.Endpoints.Games.Create;
using GameService.Endpoints.Games.UpdateStatus;

namespace GameService.Services;

public interface IGameEventPublisher
{
    Task PublishGameCreatedAsync(GameCreatedEvent evt, CancellationToken ct = default);
    Task PublishStatusUpdatedAsync(GameStatusUpdatedEvent evt, CancellationToken ct = default);
}

public class FastEndpointsGameEventPublisher : IGameEventPublisher
{
    public Task PublishGameCreatedAsync(GameCreatedEvent evt, CancellationToken ct = default)
        => evt.PublishAsync(Mode.WaitForNone, ct);

    public Task PublishStatusUpdatedAsync(GameStatusUpdatedEvent evt, CancellationToken ct = default)
        => evt.PublishAsync(Mode.WaitForNone, ct);
}

