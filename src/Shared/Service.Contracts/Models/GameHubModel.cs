namespace Service.Contracts.Models;

public record GameHubModel
{
    public required string GameId { get; init; }
    public required int CurrentPlayer { get; init; }
    public required int Winner { get; init; }
    public required bool IsDraw { get; init; }
    public required bool IsOver { get; init; }
    public required List<GameHubCell> Board { get; init; }
}

public record GameHubCell(int Row, int Col, int Mark);
