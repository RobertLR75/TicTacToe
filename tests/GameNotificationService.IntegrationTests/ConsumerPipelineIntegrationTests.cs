using GameNotificationService.Services;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Service.Contracts.Events;
using Service.Contracts.Shared;
using TicTacToe.Testing;
using Xunit;

namespace GameNotificationService.IntegrationTests;

[Collection(GameNotificationCollection.Name)]
public sealed class ConsumerPipelineIntegrationTests : GameNotificationServiceIntegrationTestBase
{
    public ConsumerPipelineIntegrationTests(GameNotificationServiceIntegrationTestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task RabbitMq_and_postgres_pipeline_persists_initialized_notification_end_to_end()
    {
        await using var provider = CreateServiceProvider(enableEventConsumers: true);

        var bus = provider.GetRequiredService<IBusControl>();
        await bus.StartAsync();
        try
        {
            var eventId = Guid.NewGuid().ToString("N");
            var publisher = provider.GetRequiredService<IPublishEndpoint>();
            await publisher.Publish(BuildInitializedEvent(eventId));

            var persisted = await AsyncPolling.WaitForAsync(async () =>
            {
                var notifications = await ListNotificationsAsync(provider, new NotificationQuery(1, 20, "game-1"));
                return notifications.Any(x => x.EventId == eventId && x.EventType == "GameStateInitialized");
            }, TimeSpan.FromSeconds(20));

            Assert.True(persisted);
        }
        finally
        {
            await bus.StopAsync();
        }
    }

    [Fact]
    public async Task RabbitMq_and_postgres_pipeline_persists_updated_notification_end_to_end()
    {
        await using var provider = CreateServiceProvider(enableEventConsumers: true);

        var bus = provider.GetRequiredService<IBusControl>();
        await bus.StartAsync();
        try
        {
            var eventId = Guid.NewGuid().ToString("N");
            var publisher = provider.GetRequiredService<IPublishEndpoint>();
            await publisher.Publish(BuildUpdatedEvent(eventId));

            var persisted = await AsyncPolling.WaitForAsync(async () =>
            {
                var notifications = await ListNotificationsAsync(provider, new NotificationQuery(1, 20, "game-1"));
                return notifications.Any(x => x.EventId == eventId && x.EventType == "GameStateUpdated");
            }, TimeSpan.FromSeconds(20));

            Assert.True(persisted);
        }
        finally
        {
            await bus.StopAsync();
        }
    }

    private GameStateInitialized BuildInitializedEvent(string? eventId = null)
        => new()
        {
            EventId = eventId ?? Guid.NewGuid().ToString("N"),
            SchemaVersion = "1.0",
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            CurrentPlayer = PlayerMarkEnum.X,
            Winner = PlayerMarkEnum.X,
            IsDraw = false,
            IsOver = false,
            Board = [new CellEventDto(0, 0, PlayerMarkEnum.X)],
            OccurredAtUtc = DateTimeOffset.UtcNow,
            CorrelationId = "corr-created"
        };

    private GameStateUpdated BuildUpdatedEvent(string? eventId = null)
        => new()
        {
            EventId = eventId ?? Guid.NewGuid().ToString("N"),
            SchemaVersion = "1.0",
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            CurrentPlayer = PlayerMarkEnum.X,
            Winner = PlayerMarkEnum.X,
            IsDraw = false,
            IsOver = false,
            Board = [new CellEventDto(0, 0, PlayerMarkEnum.O), new CellEventDto(0, 1, PlayerMarkEnum.X)],
            OccurredAtUtc = DateTimeOffset.UtcNow,
            CorrelationId = "corr-updated"
        };
}
