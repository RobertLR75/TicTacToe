namespace GameStateService.Services;

public interface IGameEventPublisher
{
    Task PublishEventAsync<T>(T @event, CancellationToken ct = default) where T : class;
}
