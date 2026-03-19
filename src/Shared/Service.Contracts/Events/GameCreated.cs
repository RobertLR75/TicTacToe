using Service.Contracts.Shared;
using SharedLibrary.Interfaces;

namespace Service.Contracts.Events;

public record GameCreated : ISharedEvent
{
    public required string EventId { get; init; }
    public Guid Id { get; set; }
    public required string SchemaVersion { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public required string Player1 { get; init; }
    public required DateTimeOffset OccurredAtUtc { get; set; }
    public string? CorrelationId { get; set; }
}