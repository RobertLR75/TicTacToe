using System.Text.Json;
using GameNotificationService.Configuration;
using GameNotificationService.Services;
using Microsoft.Extensions.Options;
using SharedLibrary.Services.Interfaces;
using GameStateInitializedEvent = Service.Contracts.Events.GameStateInitialized;

namespace GameNotificationService.Features.Notifications.Consumers;

public interface IGameStateInitializedHandler : IRequestHandler<GameStateInitializedCommand, bool>;

public sealed record GameStateInitializedCommand(GameStateInitializedEvent Message) : IRequest<bool>;

public sealed class GameStateInitializedHandler(
    INotificationStorageService notificationStorage,
    IOptions<MessagingOptions> messagingOptions,
    ISignalRGameNotificationPublisher notificationPublisher,
    ILogger<GameStateInitializedHandler> logger) : IGameStateInitializedHandler
{
    public async Task<bool> HandleAsync(GameStateInitializedCommand request, CancellationToken ct = default)
    {
        var message = request.Message;

        if (!messagingOptions.Value.EnableEventConsumers)
        {
            logger.LogDebug("Skipping GameStateInitializedEvent {EventId} because event consumers are disabled.", message.EventId);
            return false;
        }

        if (!GameNotificationMapper.TryMap(message, out var notification))
        {
            logger.LogWarning(
                "Skipping GameStateInitializedEvent {EventId} due to invalid payload. CorrelationId: {CorrelationId}",
                message.EventId,
                message.CorrelationId);
            return false;
        }

        logger.LogInformation(
            "Consumed GameStateInitializedEvent {EventId} for game {GameId}. CorrelationId: {CorrelationId}",
            message.EventId,
            message.Id,
            message.CorrelationId);

        await notificationPublisher.PublishAsync(notification!, ct);

        var inserted = await notificationStorage.TryAddAsync(new NotificationWriteModel
        {
            EventId = message.EventId,
            GameId = message.Id.ToString(),
            EventType = "GameStateInitialized",
            PayloadSummary = JsonSerializer.Serialize(new
            {
                message.CurrentPlayer,
                message.Winner,
                message.IsDraw,
                message.IsOver,
                BoardCellCount = message.Board.Count
            }),
            OccurredAtUtc = message.OccurredAtUtc,
            ReceivedAtUtc = DateTimeOffset.UtcNow
        }, ct);

        if (!inserted)
        {
            logger.LogInformation("Skipped duplicate GameStateInitializedEvent {EventId} for game {GameId}.", message.EventId, message.Id);
        }

        return inserted;
    }
}
