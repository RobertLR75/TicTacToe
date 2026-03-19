using GameNotificationService.Configuration;
using GameNotificationService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SharedLibrary.Interfaces;
using Service.Contracts.Events;
using Service.Contracts.Notifications;
using Service.Contracts.Shared;
using TicTacToe.Testing;
using Xunit;

namespace GameNotificationService.UnitTests;

public sealed class GameNotificationServiceUnitTestFixture : IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => Task.CompletedTask;

    public IOptions<MessagingOptions> CreateMessagingOptions(bool enableEventConsumers = true)
        => Options.Create(new MessagingOptions { EnableEventConsumers = enableEventConsumers });

    public IConfiguration BuildConfiguration(Dictionary<string, string?> values)
        => TestConfigurationFactory.Build(values);

    public GameStateInitialized BuildInitializedEvent(string? eventId = null, string gameId = "11111111-1111-1111-1111-111111111111")
        => new()
        {
            EventId = eventId ?? Guid.NewGuid().ToString("N"),
            SchemaVersion = "1.0",
            Id = Guid.Parse(gameId),
            CurrentPlayer = PlayerMarkEnum.X,
            Winner = PlayerMarkEnum.None,
            IsDraw = false,
            IsOver = false,
            Board = [new CellEventDto(0, 0, PlayerMarkEnum.X)],
            OccurredAtUtc = DateTimeOffset.UtcNow,
            CorrelationId = "corr-created"
        };

    public GameStateUpdated BuildUpdatedEvent(string? eventId = null, string gameId = "11111111-1111-1111-1111-111111111111")
        => new()
        {
            EventId = eventId ?? Guid.NewGuid().ToString("N"),
            SchemaVersion = "1.0",
            Id = Guid.Parse(gameId),
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

    public sealed class InMemoryNotificationRepository : INotificationStorageService
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
                Id = Guid.NewGuid(),
                EventId = notification.EventId,
                GameId = notification.GameId,
                EventType = notification.EventType,
                PayloadSummary = notification.PayloadSummary,
                OccurredAtUtc = notification.OccurredAtUtc,
                ReceivedAtUtc = notification.ReceivedAtUtc,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = null
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

        public string Name => "notifications";

        public Task<Guid> CreateAsync(NotificationRecord entity, CancellationToken cancellationToken)
        {
            Records.Add(entity);
            return Task.FromResult(entity.Id);
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            Records.RemoveAll(x => x.Id == id);
            return Task.CompletedTask;
        }

        public Task<NotificationRecord?> GetAsync(Guid id, CancellationToken cancellationToken)
            => Task.FromResult(Records.SingleOrDefault(x => x.Id == id));

        public Task<NotificationRecord> GetAsync(IPersistenceSpecification<NotificationRecord> specification, CancellationToken cancellationToken)
            => throw new NotSupportedException("Specification-based GetAsync is not used in these tests.");

        public Task<List<NotificationRecord>> SearchAsync(IPersistenceSpecification<NotificationRecord> specification, CancellationToken cancellationToken)
            => Task.FromResult(Records.ToList());

        public Task<List<NotificationRecord>> SearchAsync(SearchFilter<NotificationRecord> filter, CancellationToken cancellationToken)
            => Task.FromResult(ApplyFilters(filter.Parameters));

        public Task UpdateAsync(NotificationRecord entity, CancellationToken cancellationToken)
        {
            var index = Records.FindIndex(x => x.Id == entity.Id);
            if (index >= 0)
            {
                Records[index] = entity;
            }

            return Task.CompletedTask;
        }

        private List<NotificationRecord> ApplyFilters(IReadOnlyCollection<SearchFilterCriterion<NotificationRecord>> parameters)
        {
            if (parameters.Count == 0)
            {
                return Records.ToList();
            }

            return Records.Where(record => parameters.All(parameter =>
            {
                var compiled = parameter.FieldSelector.Compile();
                var value = compiled(record)?.ToString();
                return string.Equals(value, parameter.Value?.ToString(), StringComparison.Ordinal);
            })).ToList();
        }
    }

    public sealed class RecordingNotificationPublisher : ISignalRGameNotificationPublisher
    {
        public List<GameStateInitializedNotification> InitializedNotifications { get; } = [];
        public List<GameStateUpdatedNotification> UpdatedNotifications { get; } = [];
        
        public Task PublishAsync<TNotification>(TNotification notification, CancellationToken ct = new CancellationToken()) where TNotification : class, INotification
        {
            if (notification is GameStateUpdatedNotification updated)
                UpdatedNotifications.Add(updated);
            else if (notification is GameStateInitializedNotification initialized)
                InitializedNotifications.Add(initialized);
            
            return Task.CompletedTask;
        }
    }
}
