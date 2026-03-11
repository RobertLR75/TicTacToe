using Service.Contracts.Shared;

namespace Service.Contracts.Responses;

public record GetGameResponse
{
    public required string GameId { get; init; }
    public required PlayerMarkEnum CurrentPlayer { get; init; }
    public required PlayerMarkEnum Winner { get; init; }
    public required bool IsDraw { get; init; }
    public required bool IsOver { get; init; }
    public required List<CellDto> Board { get; init; }
}

public record CellDto(int Row, int Col, PlayerMarkEnum MarkEnum);

