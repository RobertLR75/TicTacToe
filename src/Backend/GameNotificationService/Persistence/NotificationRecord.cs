namespace GameNotificationService.Persistence;

public sealed record NotificationRecord
{
    public required string Id { get; init; }

    public required string EventId { get; init; }

    public required string GameId { get; init; }

    public required string EventType { get; init; }

    public required string PayloadSummary { get; init; }

    public required DateTimeOffset OccurredAtUtc { get; init; }

    public required DateTimeOffset ReceivedAtUtc { get; init; }
}

public sealed record NotificationWriteModel
{
    public required string EventId { get; init; }

    public required string GameId { get; init; }

    public required string EventType { get; init; }

    public required string PayloadSummary { get; init; }

    public required DateTimeOffset OccurredAtUtc { get; init; }

    public required DateTimeOffset ReceivedAtUtc { get; init; }
}

public sealed record NotificationQuery(int Page, int PageSize, string? GameId);
