using Service.Contracts.Shared;

namespace Service.Contracts.Events;

public sealed record GameStatusUpdated : ISharedEvent
{
    public required string EventId { get; init; }
    public required string SchemaVersion { get; init; }
    public Guid GameId { get; init; }
    public required GameStatusEnum NewStatus { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
    public required DateTimeOffset OccurredAtUtc { get; init; }
    public string? CorrelationId { get; init; }
}