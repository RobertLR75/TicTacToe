using System.Diagnostics;
using Service.Contracts.Events;
using Service.Contracts.Shared;

namespace GameService.Services;

public static class GameEventMapper
{
    public static GameCreated ToGameCreated(Models.Game game)
    {
        return new GameCreated
        {
            EventId = Guid.NewGuid().ToString("N"),
            SchemaVersion = EventSchemaVersion.V1,
            GameId = game.Id,
            CreatedAt = game.CreatedAt,
            Player1 = game.Player1.Id,
            OccurredAtUtc = DateTimeOffset.UtcNow,
            CorrelationId = Activity.Current?.TraceId.ToString()
        };
    }

    public static GameStatusUpdated ToGameStateUpdated(Models.Game game)
    {
        return new GameStatusUpdated
        {
            EventId = Guid.NewGuid().ToString("N"),
            SchemaVersion = EventSchemaVersion.V1,
            GameId = game.Id,
            NewStatus = (GameStatusEnum)game.Status,
            UpdatedAt = game.UpdatedAt ?? DateTimeOffset.UtcNow,
            OccurredAtUtc = DateTimeOffset.UtcNow,
            CorrelationId = Activity.Current?.TraceId.ToString()
        };
    }
}

public static class EventSchemaVersion
{
    public const string V1 = "1.0";
}