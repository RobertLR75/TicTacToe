using Service.Contracts.Shared;
using SharedLibrary.Interfaces;

namespace Service.Contracts.Notifications;

public record GameStateUpdatedNotification : INotification
{
    public required PlayerMarkEnum CurrentPlayer { get; init; }
    public required PlayerMarkEnum Winner { get; init; }
    public required bool IsDraw { get; init; }
    public required bool IsOver { get; init; }
    public required List<CellNotification> Board { get; init; }
    public required string Id { get; set; }
}

public record GameStateInitializedNotification : INotification
{
    public required PlayerMarkEnum CurrentPlayer { get; init; }
    public required PlayerMarkEnum Winner { get; init; }
    public required bool IsDraw { get; init; }
    public required bool IsOver { get; init; }
    public required List<CellNotification> Board { get; init; }
    public required string Id { get; set; }
}

public record CellNotification(int Row, int Col, PlayerMarkEnum Mark);
