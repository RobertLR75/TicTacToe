namespace Service.Contracts.GameState;

public record GameStateInitialized
{
    public required string EventId { get; init; }
    public required string SchemaVersion { get; init; }
    public required string GameId { get; init; }
    public required int CurrentPlayer { get; init; }
    public required int Winner { get; init; }
    public required bool IsDraw { get; init; }
    public required bool IsOver { get; init; }
    public required List<CellEventDto> Board { get; init; }
    public required DateTimeOffset OccurredAtUtc { get; init; }
    public string? CorrelationId { get; init; }
}