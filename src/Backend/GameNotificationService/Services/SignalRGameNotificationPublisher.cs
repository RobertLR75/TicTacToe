using GameNotificationService.Hubs;
using GameNotificationService.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace GameNotificationService.Services;

public class SignalRGameNotificationPublisher(
    IHubContext<GameHub> hubContext,
    ILogger<SignalRGameNotificationPublisher> logger) : IGameNotificationPublisher
{
    public async Task PublishGameStateInitializedAsync(GameStateInitializedNotification notification, CancellationToken ct = default)
    {
        logger.LogInformation("Publishing GameStateInitializedNotification for game {GameId}", notification.GameId);

        await hubContext.Clients.Group(notification.GameId)
            .SendAsync("GameStateInitializedNotification", notification, ct);
    }

    public async Task PublishGameStateUpdatedAsync(GameStateUpdatedNotification notification, CancellationToken ct = default)
    {
        logger.LogInformation("Publishing GameStateUpdatedNotification for game {GameId}", notification.GameId);

        await hubContext.Clients.Group(notification.GameId)
            .SendAsync("GameStateUpdatedNotification", notification, ct);
    }
}
