using Service.Contracts.Shared;
using SharedLibrary.Interfaces;

namespace Service.Contracts.Events;

public record GameStateInitialized : ISharedEvent
{
    public required string EventId { get; init; }
    public Guid Id { get; set; }
    public required string SchemaVersion { get; init; }
    public required PlayerMarkEnum CurrentPlayer { get; init; }
    public required PlayerMarkEnum Winner { get; init; }
    public required bool IsDraw { get; init; }
    public required bool IsOver { get; init; }
    public required List<CellEventDto> Board { get; init; }
    public required DateTimeOffset OccurredAtUtc { get; set; }
    public string? CorrelationId { get; set; }
}