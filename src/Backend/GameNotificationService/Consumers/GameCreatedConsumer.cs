using System.Text.Json;
using GameNotificationService.Configuration;
using GameNotificationService.Persistence;
using GameNotificationService.Services;
using GameStateService.Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Options;

namespace GameNotificationService.Consumers;

public sealed class GameCreatedConsumer(
    // INotificationRepository notificationRepository,
    IOptions<MessagingOptions> messagingOptions,
    IGameNotificationPublisher notificationPublisher,
    ILogger<GameCreatedConsumer> logger) : IConsumer<GameCreatedEvent>
{
    public Task Consume(ConsumeContext<GameCreatedEvent> context)
    {
        return ProcessAsync(context.Message, context.CancellationToken);
    }

    internal async Task ProcessAsync(GameCreatedEvent message, CancellationToken ct)
    {
        if (!messagingOptions.Value.EnableEventConsumers)
        {
            logger.LogDebug("Skipping GameCreatedEvent {EventId} because event consumers are disabled.", message.EventId);
            return;
        }

        if (!GameNotificationMapper.TryMap(message, out var notification))
        {
            logger.LogWarning(
                "Skipping GameCreatedEvent {EventId} due to invalid payload. CorrelationId: {CorrelationId}",
                message.EventId,
                message.CorrelationId);
            return;
        }

        logger.LogInformation(
            "Consumed GameCreatedEvent {EventId} for game {GameId}. CorrelationId: {CorrelationId}",
            message.EventId,
            message.GameId,
            message.CorrelationId);

        await notificationPublisher.PublishGameCreatedAsync(notification!, ct);

        var writeModel = new NotificationWriteModel
        {
            EventId = message.EventId,
            GameId = message.GameId,
            EventType = "GameCreated",
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

        // var inserted = await notificationRepository.TryAddAsync(writeModel, ct);
        // if (!inserted)
        // {
        //     logger.LogInformation("Skipped duplicate GameCreatedEvent {EventId} for game {GameId}.", message.EventId, message.GameId);
        // }
    }
}
