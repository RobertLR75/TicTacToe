using Service.Contracts.Shared;

namespace Service.Contracts.Events;

public record GameStateInitialized : ISharedEvent
{
    public required string EventId { get; init; }
    public required string SchemaVersion { get; init; }
    public required string GameId { get; init; }
    public required PlayerMarkEnum CurrentPlayer { get; init; }
    public required PlayerMarkEnum Winner { get; init; }
    public required bool IsDraw { get; init; }
    public required bool IsOver { get; init; }
    public required List<CellEventDto> Board { get; init; }
    public required DateTimeOffset OccurredAtUtc { get; init; }
    public string? CorrelationId { get; init; }
}