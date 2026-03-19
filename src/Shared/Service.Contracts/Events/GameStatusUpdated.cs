using Service.Contracts.Shared;
using SharedLibrary.Interfaces;

namespace Service.Contracts.Events;

public sealed record GameStatusUpdated : ISharedEvent
{
    public required string EventId { get; init; }
    public Guid Id { get; set; }
    public required string SchemaVersion { get; init; }
    public required GameStatusEnum NewStatus { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
    public required DateTimeOffset OccurredAtUtc { get; set; }
    public string? CorrelationId { get; set; }
}