using GameStateService.Models;
using GameStateService.Services;

namespace GameStateService.Endpoints.Games.MakeMove;

public sealed record GameLogicMoveRequest(GameState Game, int Row, int Col) : IRequest<GameLogicMoveResult>;

public sealed record GameLogicMoveResult
{
    public required GameLogicMoveStatus Status { get; init; }

    public static GameLogicMoveResult Success() => new() { Status = GameLogicMoveStatus.Success };

    public static GameLogicMoveResult GameOver() => new() { Status = GameLogicMoveStatus.GameOver };

    public static GameLogicMoveResult CellOccupied() => new() { Status = GameLogicMoveStatus.CellOccupied };
}

public sealed class GameLogicMoveRequestHandler(
    IRequestHandler<CheckWinnerRequest, CheckWinnerResult> checkWinnerHandler,
    IRequestHandler<CheckDrawRequest, CheckDrawResult> checkDrawHandler)
    : IRequestHandler<GameLogicMoveRequest, GameLogicMoveResult>
{
    public async Task<GameLogicMoveResult> HandleAsync(GameLogicMoveRequest request, CancellationToken ct = default)
    {
        if (request.Game.IsOver)
        {
            return GameLogicMoveResult.GameOver();
        }

        if (!request.Game.Board.IsEmpty(request.Row, request.Col))
        {
            return GameLogicMoveResult.CellOccupied();
        }

        request.Game.Board.SetCell(request.Row, request.Col, request.Game.CurrentPlayer);

        var winnerResult = await checkWinnerHandler.HandleAsync(new CheckWinnerRequest(request.Game.Board), ct);
        if (winnerResult.Winner != PlayerMark.None)
        {
            request.Game.Winner = winnerResult.Winner;
        }
        else
        {
            var drawResult = await checkDrawHandler.HandleAsync(new CheckDrawRequest(request.Game.Board), ct);
            if (drawResult.IsDraw)
            {
                request.Game.IsDraw = true;
            }
            else
            {
                request.Game.CurrentPlayer = request.Game.CurrentPlayer == PlayerMark.X
                    ? PlayerMark.O
                    : PlayerMark.X;
            }
        }

        return GameLogicMoveResult.Success();
    }
}

public enum GameLogicMoveStatus
{
    Success,
    GameOver,
    CellOccupied
}
