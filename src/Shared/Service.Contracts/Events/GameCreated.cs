using Service.Contracts.Shared;

namespace Service.Contracts.Events;

public record GameCreated : ISharedEvent
{
    public required string EventId { get; init; }
    public required string SchemaVersion { get; init; }
    public Guid GameId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public required string Player1 { get; init; }
    public required DateTimeOffset OccurredAtUtc { get; init; }
    public string? CorrelationId { get; init; }
}