using Service.Contracts.Shared;

namespace Service.Contracts.Responses;

public abstract record GameStateResponse
{
    public required string GameId { get; init; }
    public required PlayerMarkEnum CurrentPlayer { get; init; }
    public required PlayerMarkEnum Winner { get; init; }
    public required bool IsDraw { get; init; }
    public required bool IsOver { get; init; }
    public required List<GameStateCellResponse> Board { get; init; }
}

public sealed record GetGameStateResponse : GameStateResponse;

public sealed record UpdateGameStateResponse : GameStateResponse;

public sealed record GameStateCellResponse(int Row, int Col, PlayerMarkEnum Mark);
