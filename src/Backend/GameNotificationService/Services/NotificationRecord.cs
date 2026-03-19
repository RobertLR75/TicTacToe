using SharedLibrary.Interfaces;

namespace GameNotificationService.Services;

public sealed record NotificationRecord : IEntity
{
    public Guid Id { get; set; }

    public required string EventId { get; init; }

    public required string GameId { get; init; }

    public required string EventType { get; init; }

    public required string PayloadSummary { get; init; }

    public required DateTimeOffset OccurredAtUtc { get; set; }

    public required DateTimeOffset ReceivedAtUtc { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
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
