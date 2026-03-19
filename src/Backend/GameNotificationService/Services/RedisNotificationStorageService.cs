using Microsoft.Extensions.Caching.Distributed;
using SharedLibrary.Interfaces;
using SharedLibrary.Redis;

namespace GameNotificationService.Services;

public sealed class RedisNotificationStorageService(IDistributedCache cache) : BaseRedisPersistenceService<NotificationRecord>(cache), INotificationStorageService
{
    public override string Name => "Notifications";

    public async Task<bool> TryAddAsync(NotificationWriteModel notification, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(notification);

        var filter = new SearchFilter<NotificationRecord>();
        filter.Parameters.Add(new(notification => notification.EventId, notification.EventId));

        var existing = await SearchAsync(filter, ct);
        if (existing.Count > 0)
        {
            return false;
        }

        var record = new NotificationRecord
        {
            Id = Guid.CreateVersion7(),
            EventId = notification.EventId,
            GameId = notification.GameId,
            EventType = notification.EventType,
            PayloadSummary = notification.PayloadSummary,
            OccurredAtUtc = notification.OccurredAtUtc,
            ReceivedAtUtc = notification.ReceivedAtUtc,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = null
        };

        await CreateAsync(record, ct);
        return true;
    }

    public async Task<IReadOnlyList<NotificationRecord>> ListAsync(NotificationQuery query, CancellationToken ct = default)
    {
        var filter = new SearchFilter<NotificationRecord>();
        if (!string.IsNullOrWhiteSpace(query.GameId))
        {
            filter.Parameters.Add(new(notification => notification.GameId, query.GameId));
        }

        var notifications = await SearchAsync(filter, ct);

        return notifications
            .OrderByDescending(x => x.OccurredAtUtc)
            .ThenByDescending(x => x.ReceivedAtUtc)
            .Skip((Math.Max(query.Page, 1) - 1) * Math.Max(query.PageSize, 1))
            .Take(Math.Max(query.PageSize, 1))
            .ToList();
    }
}
