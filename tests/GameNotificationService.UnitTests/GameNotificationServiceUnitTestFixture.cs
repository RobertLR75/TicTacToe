using GameNotificationService.Configuration;
using GameNotificationService.Persistence;
using GameNotificationService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Service.Contracts.Events;
using Service.Contracts.Notifications;
using Service.Contracts.Shared;
using TicTacToe.Testing;
using Xunit;
using YamlDotNet.Core;

namespace GameNotificationService.UnitTests;

public sealed class GameNotificationServiceUnitTestFixture : IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => Task.CompletedTask;

    public IOptions<MessagingOptions> CreateMessagingOptions(bool enableEventConsumers = true)
        => Options.Create(new MessagingOptions { EnableEventConsumers = enableEventConsumers });

    public IConfiguration BuildConfiguration(Dictionary<string, string?> values)
        => TestConfigurationFactory.Build(values);

    public GameStateInitialized BuildInitializedEvent(string? eventId = null, string gameId = "game-1")
        => new()
        {
            EventId = eventId ?? Guid.NewGuid().ToString("N"),
            SchemaVersion = "1.0",
            GameId = gameId,
            CurrentPlayer = PlayerMarkEnum.X,
            Winner = PlayerMarkEnum.None,
            IsDraw = false,
            IsOver = false,
            Board = [new CellEventDto(0, 0, PlayerMarkEnum.X)],
            OccurredAtUtc = DateTimeOffset.UtcNow,
            CorrelationId = "corr-created"
        };

    public GameStateUpdated BuildUpdatedEvent(string? eventId = null, string gameId = "game-1")
        => new()
        {
            EventId = eventId ?? Guid.NewGuid().ToString("N"),
            SchemaVersion = "1.0",
            GameId = gameId,
            CurrentPlayer = PlayerMarkEnum.X,
            Winner = PlayerMarkEnum.X,
            IsDraw = false,
            IsOver = false,
            Board = [new CellEventDto(0, 0, PlayerMarkEnum.O), new CellEventDto(0, 1, PlayerMarkEnum.X)],
            OccurredAtUtc = DateTimeOffset.UtcNow,
            CorrelationId = "corr-updated"
        };

    public InMemoryNotificationRepository CreateRepository(bool rejectDuplicates = false)
        => new() { RejectDuplicates = rejectDuplicates };

    public RecordingNotificationPublisher CreatePublisher()
        => new();

    public sealed class InMemoryNotificationRepository : INotificationRepository
    {
        private readonly HashSet<string> _eventIds = [];
        public List<NotificationWriteModel> Writes { get; } = [];
        public bool RejectDuplicates { get; init; }
        public List<NotificationRecord> Records { get; } = [];

        public Task<bool> TryAddAsync(NotificationWriteModel notification, CancellationToken ct = default)
        {
            if (RejectDuplicates && !_eventIds.Add(notification.EventId))
                return Task.FromResult(false);

            _eventIds.Add(notification.EventId);
            Writes.Add(notification);
            Records.Add(new NotificationRecord
            {
                Id = Guid.NewGuid().ToString("N"),
                EventId = notification.EventId,
                GameId = notification.GameId,
                EventType = notification.EventType,
                PayloadSummary = notification.PayloadSummary,
                OccurredAtUtc = notification.OccurredAtUtc,
                ReceivedAtUtc = notification.ReceivedAtUtc
            });
            return Task.FromResult(true);
        }

        public Task<IReadOnlyList<NotificationRecord>> ListAsync(NotificationQuery query, CancellationToken ct = default)
        {
            var notifications = Records
                .Where(x => query.GameId is null || x.GameId == query.GameId)
                .OrderByDescending(x => x.OccurredAtUtc)
                .ThenByDescending(x => x.ReceivedAtUtc)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            return Task.FromResult<IReadOnlyList<NotificationRecord>>(notifications);
        }
    }

    public sealed class RecordingNotificationPublisher : IGameNotificationPublisher
    {
        public List<GameStateInitializedNotification> InitializedNotifications { get; } = [];
        public List<GameStateUpdatedNotification> UpdatedNotifications { get; } = [];

        public Task PublishGameStateInitializedAsync(GameStateInitializedNotification notification, CancellationToken ct = default)
        {
            InitializedNotifications.Add(notification);
            return Task.CompletedTask;
        }

        public Task PublishGameStateUpdatedAsync(GameStateUpdatedNotification notification, CancellationToken ct = default)
        {
            UpdatedNotifications.Add(notification);
            return Task.CompletedTask;
        }
    }
}
