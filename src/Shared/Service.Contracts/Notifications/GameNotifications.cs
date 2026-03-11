using Service.Contracts.Shared;

namespace Service.Contracts.Notifications;

public record GameStateUpdatedNotification
{
    public required string GameId { get; init; }
    public required PlayerMarkEnum CurrentPlayer { get; init; }
    public required PlayerMarkEnum Winner { get; init; }
    public required bool IsDraw { get; init; }
    public required bool IsOver { get; init; }
    public required List<CellNotification> Board { get; init; }
}

public record GameStateInitializedNotification
{
    public required string GameId { get; init; }
    public required PlayerMarkEnum CurrentPlayer { get; init; }
    public required PlayerMarkEnum Winner { get; init; }
    public required bool IsDraw { get; init; }
    public required bool IsOver { get; init; }
    public required List<CellNotification> Board { get; init; }
}

public record CellNotification(int Row, int Col, PlayerMarkEnum Mark);
