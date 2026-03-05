namespace TicTacToeMud.Models;

public record GameResponse
{
    public required string GameId { get; init; }
    public required int CurrentPlayer { get; init; }
    public required int Winner { get; init; }
    public required bool IsDraw { get; init; }
    public required bool IsOver { get; init; }
    public required List<CellDto> Board { get; init; }
}

public record CellDto(int Row, int Col, int Mark);
