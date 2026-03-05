using GameNotificationService.Notifications;

namespace GameNotificationService.Services;

public interface IGameNotificationPublisher
{
    Task PublishGameCreatedAsync(GameCreatedNotification notification, CancellationToken ct = default);

    Task PublishGameStateUpdatedAsync(GameStateUpdatedNotification notification, CancellationToken ct = default);
}
