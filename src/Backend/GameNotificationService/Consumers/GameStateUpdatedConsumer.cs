using System.Text.Json;
using GameNotificationService.Configuration;
using GameNotificationService.Persistence;
using GameNotificationService.Services;
using MassTransit;
using Microsoft.Extensions.Options;
using Service.Contracts.GameState;

namespace GameNotificationService.Consumers;

public sealed class GameStateUpdatedConsumer(
    INotificationRepository notificationRepository,
    IOptions<MessagingOptions> messagingOptions,
    IGameNotificationPublisher notificationPublisher,
    ILogger<GameStateUpdatedConsumer> logger) : IConsumer<GameStateUpdated>
{
    public Task Consume(ConsumeContext<GameStateUpdated> context)
    {
        return ProcessAsync(context.Message, context.CancellationToken);
    }

    internal async Task ProcessAsync(GameStateUpdated message, CancellationToken ct)
    {
        if (!messagingOptions.Value.EnableEventConsumers)
        {
            logger.LogDebug("Skipping GameStateUpdatedEvent {EventId} because event consumers are disabled.", message.EventId);
            return;
        }

        if (!GameNotificationMapper.TryMap(message, out var notification))
        {
            logger.LogWarning(
                "Skipping GameStateUpdatedEvent {EventId} due to invalid payload. CorrelationId: {CorrelationId}",
                message.EventId,
                message.CorrelationId);
            return;
        }

        logger.LogInformation(
            "Consumed GameStateUpdatedEvent {EventId} for game {GameId}. CorrelationId: {CorrelationId}",
            message.EventId,
            message.GameId,
            message.CorrelationId);

        await notificationPublisher.PublishGameStateUpdatedAsync(notification!, ct);

        var writeModel = new NotificationWriteModel
        {
            EventId = message.EventId,
            GameId = message.GameId,
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
        };

        var inserted = await notificationRepository.TryAddAsync(writeModel, ct);
        if (!inserted)
        {
            logger.LogInformation("Skipped duplicate GameStateUpdatedEvent {EventId} for game {GameId}.", message.EventId, message.GameId);
        }
    }
}
