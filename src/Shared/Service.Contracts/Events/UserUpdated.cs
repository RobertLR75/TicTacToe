using Service.Contracts.Shared;
using SharedLibrary.Interfaces;

namespace Service.Contracts.Events;

public sealed record UserUpdated : ISharedEvent
{
    public required string EventId { get; init; }
    public Guid Id { get; set; }
    public required string SchemaVersion { get; init; }
    public required Guid UserId { get; init; }
    public required string Name { get; init; }
    public required UserStatusEnum Status { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
    public required DateTimeOffset OccurredAtUtc { get; set; }
    public string? CorrelationId { get; set; }
}
