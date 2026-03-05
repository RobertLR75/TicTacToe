using GameStateService.Models;

namespace GameStateService.Contracts.Events;

public static class EventSchemaVersion
{
    public const string V1 = "1.0";
}

public record GameCreatedEvent
{
    public required string EventId { get; init; }
    public required string SchemaVersion { get; init; }
    public required string GameId { get; init; }
    public required PlayerMark CurrentPlayer { get; init; }
    public required PlayerMark Winner { get; init; }
    public required bool IsDraw { get; init; }
    public required bool IsOver { get; init; }
    public required List<CellEventDto> Board { get; init; }
    public required DateTimeOffset OccurredAtUtc { get; init; }
    public string? CorrelationId { get; init; }
}

public record GameStateUpdatedEvent
{
    public required string EventId { get; init; }
    public required string SchemaVersion { get; init; }
    public required string GameId { get; init; }
    public required PlayerMark CurrentPlayer { get; init; }
    public required PlayerMark Winner { get; init; }
    public required bool IsDraw { get; init; }
    public required bool IsOver { get; init; }
    public required List<CellEventDto> Board { get; init; }
    public required DateTimeOffset OccurredAtUtc { get; init; }
    public string? CorrelationId { get; init; }
}

public record CellEventDto(int Row, int Col, PlayerMark Mark);
