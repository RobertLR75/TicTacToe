using GameService.Models;

namespace GameService.Endpoints.Games.MakeMove;

public record MakeMoveResponse
{
    public required string GameId { get; init; }
    public required PlayerMark CurrentPlayer { get; init; }
    public required PlayerMark Winner { get; init; }
    public required bool IsDraw { get; init; }
    public required bool IsOver { get; init; }
    public required List<CellDto> Board { get; init; }
}

public record CellDto(int Row, int Col, PlayerMark Mark);

