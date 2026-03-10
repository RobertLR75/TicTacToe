namespace GameNotificationService.Notifications;

public record GameStateUpdatedNotification
{
    public required string GameId { get; init; }
    public required int CurrentPlayer { get; init; }
    public required int Winner { get; init; }
    public required bool IsDraw { get; init; }
    public required bool IsOver { get; init; }
    public required List<CellNotification> Board { get; init; }
}

public record GameStateInitializedNotification
{
    public required string GameId { get; init; }
    public required int CurrentPlayer { get; init; }
    public required int Winner { get; init; }
    public required bool IsDraw { get; init; }
    public required bool IsOver { get; init; }
    public required List<CellNotification> Board { get; init; }
}

public record CellNotification(int Row, int Col, int Mark);
