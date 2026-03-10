using GameNotificationService.Notifications;

namespace GameNotificationService.Services;

public interface IGameNotificationPublisher
{
    Task PublishGameStateInitializedAsync(GameStateInitializedNotification notification, CancellationToken ct = default);

    Task PublishGameStateUpdatedAsync(GameStateUpdatedNotification notification, CancellationToken ct = default);
}
