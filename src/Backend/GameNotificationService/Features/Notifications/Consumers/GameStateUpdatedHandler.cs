using System.Text.Json;
using GameNotificationService.Configuration;
using GameNotificationService.Services;
using Microsoft.Extensions.Options;
using SharedLibrary.Services.Interfaces;
using GameStateUpdatedEvent = Service.Contracts.Events.GameStateUpdated;

namespace GameNotificationService.Features.Notifications.Consumers;

public interface IGameStateUpdatedHandler : IRequestHandler<GameStateUpdatedCommand, bool>;

public sealed record GameStateUpdatedCommand(GameStateUpdatedEvent Message) : IRequest<bool>;

public sealed class GameStateUpdatedHandler(
    INotificationStorageService notificationStorage,
    IOptions<MessagingOptions> messagingOptions,
    ISignalRGameNotificationPublisher notificationPublisher,
    ILogger<GameStateUpdatedHandler> logger) : IGameStateUpdatedHandler
{
    public async Task<bool> HandleAsync(GameStateUpdatedCommand request, CancellationToken ct = default)
    {
        var message = request.Message;

        if (!messagingOptions.Value.EnableEventConsumers)
        {
            logger.LogDebug("Skipping GameStateUpdatedEvent {EventId} because event consumers are disabled.", message.EventId);
            return false;
        }

        if (!GameNotificationMapper.TryMap(message, out var notification))
        {
            logger.LogWarning(
                "Skipping GameStateUpdatedEvent {EventId} due to invalid payload. CorrelationId: {CorrelationId}",
                message.EventId,
                message.CorrelationId);
            return false;
        }

        if (notification is null)
        {
            return false;
        }

        logger.LogInformation(
            "Consumed GameStateUpdatedEvent {EventId} for game {GameId}. CorrelationId: {CorrelationId}",
            message.EventId,
            message.Id,
            message.CorrelationId);

        await notificationPublisher.PublishAsync(notification, ct).ConfigureAwait(false);

        var inserted = await notificationStorage.TryAddAsync(new NotificationWriteModel
        {
            EventId = message.EventId,
            GameId = message.Id.ToString(),
            EventType = "GameStateUpdated",
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
            logger.LogInformation("Skipped duplicate GameStateUpdatedEvent {EventId} for game {GameId}.", message.EventId, message.Id);
        }

        return inserted;
    }
}
