using Npgsql;
using NpgsqlTypes;

namespace GameNotificationService.Persistence;

public sealed class PostgresNotificationRepository(string connectionString) : INotificationRepository
{
    public async Task<bool> TryAddAsync(NotificationWriteModel notification, CancellationToken ct = default)
    {
        const string sql = """
                           INSERT INTO notifications
                               (id, event_id, game_id, event_type, payload_summary, occurred_at_utc, received_at_utc)
                           VALUES
                               (@id, @eventId, @gameId, @eventType, @payloadSummary::jsonb, @occurredAtUtc, @receivedAtUtc)
                           ON CONFLICT (event_id) DO NOTHING;
                           """;

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(ct);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", Guid.NewGuid().ToString("N"));
        command.Parameters.AddWithValue("eventId", notification.EventId);
        command.Parameters.AddWithValue("gameId", notification.GameId);
        command.Parameters.AddWithValue("eventType", notification.EventType);
        command.Parameters.Add("payloadSummary", NpgsqlDbType.Jsonb).Value = notification.PayloadSummary;
        command.Parameters.AddWithValue("occurredAtUtc", notification.OccurredAtUtc);
        command.Parameters.AddWithValue("receivedAtUtc", notification.ReceivedAtUtc);

        var written = await command.ExecuteNonQueryAsync(ct);
        return written > 0;
    }

    public async Task<IReadOnlyList<NotificationRecord>> ListAsync(NotificationQuery query, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT id, event_id, game_id, event_type, payload_summary, occurred_at_utc, received_at_utc
                           FROM notifications
                           WHERE (@gameId IS NULL OR game_id = @gameId)
                           ORDER BY occurred_at_utc DESC, received_at_utc DESC
                           LIMIT @limit OFFSET @offset;
                           """;

        var offset = (query.Page - 1) * query.PageSize;

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(ct);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add("gameId", NpgsqlDbType.Text).Value = (object?)query.GameId ?? DBNull.Value;
        command.Parameters.AddWithValue("limit", query.PageSize);
        command.Parameters.AddWithValue("offset", offset);

        var notifications = new List<NotificationRecord>();

        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            notifications.Add(new NotificationRecord
            {
                Id = reader.GetString(0),
                EventId = reader.GetString(1),
                GameId = reader.GetString(2),
                EventType = reader.GetString(3),
                PayloadSummary = reader.GetString(4),
                OccurredAtUtc = reader.GetFieldValue<DateTimeOffset>(5),
                ReceivedAtUtc = reader.GetFieldValue<DateTimeOffset>(6)
            });
        }

        return notifications;
    }
}
