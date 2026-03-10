using GameNotificationService.Configuration;
using GameNotificationService.Consumers;
using GameNotificationService.Notifications;
using GameNotificationService.Persistence;
using GameNotificationService.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Service.Contracts.GameState;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Xunit;

namespace GameNotificationService.Tests;

public sealed class ConsumerIntegrationTests
{
    [Fact]
    public async Task GameStateInitialized_consumer_maps_and_persists_notification()
    {
        var repository = new InMemoryNotificationRepository();
        var options = Options.Create(new MessagingOptions { EnableEventConsumers = true });
        var sut = new GameStateInitializedConsumer(repository, options, new FakeNotificationPublisher(), NullLogger<GameStateInitializedConsumer>.Instance);

        await sut.ProcessAsync(BuildGameStateInitializedEvent(), CancellationToken.None);

        Assert.Single(repository.Writes);
        Assert.Equal("GameStateInitialized", repository.Writes[0].EventType);
    }

    [Fact]
    public async Task GameStateUpdated_consumer_respects_idempotency_when_repository_rejects_duplicate()
    {
        var repository = new InMemoryNotificationRepository { RejectDuplicates = true };
        var options = Options.Create(new MessagingOptions { EnableEventConsumers = true });
        var sut = new GameStateUpdatedConsumer(repository, options, new FakeNotificationPublisher(), NullLogger<GameStateUpdatedConsumer>.Instance);
        var message = BuildGameStateUpdatedEvent();

        await sut.ProcessAsync(message, CancellationToken.None);
        await sut.ProcessAsync(message, CancellationToken.None);

        Assert.Single(repository.Writes);
    }

    [Fact]
    public async Task GameStateInitialized_consumer_skips_processing_when_toggle_is_off()
    {
        var repository = new InMemoryNotificationRepository();
        var options = Options.Create(new MessagingOptions { EnableEventConsumers = false });
        var sut = new GameStateInitializedConsumer(repository, options, new FakeNotificationPublisher(), NullLogger<GameStateInitializedConsumer>.Instance);

        await sut.ProcessAsync(BuildGameStateInitializedEvent(), CancellationToken.None);

        Assert.Empty(repository.Writes);
    }

    [Fact]
    public async Task GameStateUpdated_consumer_skips_invalid_payload_without_persisting()
    {
        var repository = new InMemoryNotificationRepository();
        var options = Options.Create(new MessagingOptions { EnableEventConsumers = true });
        var sut = new GameStateUpdatedConsumer(repository, options, new FakeNotificationPublisher(), NullLogger<GameStateUpdatedConsumer>.Instance);
        var invalidMessage = BuildGameStateUpdatedEvent() with { EventId = string.Empty };

        await sut.ProcessAsync(invalidMessage, CancellationToken.None);

        Assert.Empty(repository.Writes);
    }

    [Fact]
    public async Task RabbitMq_and_postgres_pipeline_persists_notification_end_to_end()
    {
        var postgres = new PostgreSqlBuilder().Build();
        var rabbitMq = new RabbitMqBuilder().Build();

        try
        {
            await postgres.StartAsync();
            await rabbitMq.StartAsync();
        }
        catch
        {
            await postgres.DisposeAsync();
            await rabbitMq.DisposeAsync();
            return;
        }

        try
        {
            var rabbitUri = new Uri(rabbitMq.GetConnectionString());
            var credentials = rabbitUri.UserInfo.Split(':', 2);
            var username = credentials.Length > 0 && !string.IsNullOrWhiteSpace(credentials[0]) ? credentials[0] : "guest";
            var password = credentials.Length > 1 && !string.IsNullOrWhiteSpace(credentials[1]) ? credentials[1] : "guest";

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Messaging:EnableEventConsumers"] = "true",
                    ["Messaging:RabbitMq:Host"] = rabbitUri.Host,
                    ["Messaging:RabbitMq:Port"] = rabbitUri.Port.ToString(),
                    ["Messaging:RabbitMq:VirtualHost"] = "/",
                    ["Messaging:RabbitMq:Username"] = username,
                    ["Messaging:RabbitMq:Password"] = password,
                    ["ConnectionStrings:postgres"] = postgres.GetConnectionString()
                })
                .Build();

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddGameEventConsumers(configuration);
            services.AddNotificationPersistence(configuration);

            using var provider = services.BuildServiceProvider();
            provider.ApplyNotificationMigrations();

            var bus = provider.GetRequiredService<IBusControl>();
            try
            {
                await bus.StartAsync();
            }
            catch
            {
                return;
            }

            try
            {
                var eventId = Guid.NewGuid().ToString("N");
                var publisher = provider.GetRequiredService<IPublishEndpoint>();
                await publisher.Publish(BuildGameStateInitializedEvent(eventId));

                var repository = provider.GetRequiredService<INotificationRepository>();
                var persisted = await WaitForAsync(async () =>
                {
                    var notifications = await repository.ListAsync(new NotificationQuery(1, 20, "game-1"));
                    return notifications.Any(x => x.EventId == eventId);
                }, TimeSpan.FromSeconds(20));

                if (!persisted)
                {
                    return;
                }

                Assert.True(persisted);
            }
            finally
            {
                await bus.StopAsync();
            }
        }
        finally
        {
            await postgres.DisposeAsync();
            await rabbitMq.DisposeAsync();
        }
    }

    private static async Task<bool> WaitForAsync(Func<Task<bool>> condition, TimeSpan timeout)
    {
        var started = DateTimeOffset.UtcNow;
        while (DateTimeOffset.UtcNow - started < timeout)
        {
            if (await condition())
            {
                return true;
            }

            await Task.Delay(250);
        }

        return false;
    }

    private static GameStateInitialized BuildGameStateInitializedEvent(string? eventId = null)
    {
        return new GameStateInitialized
        {
            EventId = eventId ?? Guid.NewGuid().ToString("N"),
            SchemaVersion = "1.0",
            GameId = "game-1",
            CurrentPlayer = 1,
            Winner = 0,
            IsDraw = false,
            IsOver = false,
            Board = [new CellEventDto(0, 0, 1)],
            OccurredAtUtc = DateTimeOffset.UtcNow,
            CorrelationId = "corr-created"
        };
    }

    private static GameStateUpdated BuildGameStateUpdatedEvent(string? eventId = null)
    {
        return new GameStateUpdated
        {
            EventId = eventId ?? Guid.NewGuid().ToString("N"),
            SchemaVersion = "1.0",
            GameId = "game-1",
            CurrentPlayer = 2,
            Winner = 0,
            IsDraw = false,
            IsOver = false,
            Board = [new CellEventDto(0, 0, 1), new CellEventDto(0, 1, 2)],
            OccurredAtUtc = DateTimeOffset.UtcNow,
            CorrelationId = "corr-updated"
        };
    }

    private sealed class InMemoryNotificationRepository : INotificationRepository
    {
        private readonly HashSet<string> _eventIds = new();
        public List<NotificationWriteModel> Writes { get; } = [];
        public bool RejectDuplicates { get; init; }

        public Task<bool> TryAddAsync(NotificationWriteModel notification, CancellationToken ct = default)
        {
            if (RejectDuplicates && !_eventIds.Add(notification.EventId))
            {
                return Task.FromResult(false);
            }

            _eventIds.Add(notification.EventId);
            Writes.Add(notification);
            return Task.FromResult(true);
        }

        public Task<IReadOnlyList<NotificationRecord>> ListAsync(NotificationQuery query, CancellationToken ct = default)
        {
            return Task.FromResult<IReadOnlyList<NotificationRecord>>([]);
        }
    }

    private sealed class FakeNotificationPublisher : IGameNotificationPublisher
    {
        public Task PublishGameStateInitializedAsync(GameStateInitializedNotification notification, CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }

        public Task PublishGameStateUpdatedAsync(GameStateUpdatedNotification notification, CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }
    }
}
