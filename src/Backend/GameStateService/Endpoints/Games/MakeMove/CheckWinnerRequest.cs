using GameStateService.Models;
using GameStateService.Services;

namespace GameStateService.Endpoints.Games.MakeMove;

public sealed record CheckWinnerRequest(Board Board) : IRequest<CheckWinnerResult>;

public sealed record CheckWinnerResult
{
    public required PlayerMark Winner { get; init; }

    public static CheckWinnerResult None() => new() { Winner = PlayerMark.None };

    public static CheckWinnerResult Found(PlayerMark winner) => new() { Winner = winner };
}

public sealed class CheckWinnerRequestHandler : IRequestHandler<CheckWinnerRequest, CheckWinnerResult>
{
    public Task<CheckWinnerResult> HandleAsync(CheckWinnerRequest request, CancellationToken ct = default)
    {
        for (int i = 0; i < 3; i++)
        {
            if (AllMatch(request.Board.GetCell(i, 0).Mark, request.Board.GetCell(i, 1).Mark, request.Board.GetCell(i, 2).Mark))
            {
                return Task.FromResult(CheckWinnerResult.Found(request.Board.GetCell(i, 0).Mark));
            }

            if (AllMatch(request.Board.GetCell(0, i).Mark, request.Board.GetCell(1, i).Mark, request.Board.GetCell(2, i).Mark))
            {
                return Task.FromResult(CheckWinnerResult.Found(request.Board.GetCell(0, i).Mark));
            }
        }

        if (AllMatch(request.Board.GetCell(0, 0).Mark, request.Board.GetCell(1, 1).Mark, request.Board.GetCell(2, 2).Mark))
        {
            return Task.FromResult(CheckWinnerResult.Found(request.Board.GetCell(0, 0).Mark));
        }

        if (AllMatch(request.Board.GetCell(0, 2).Mark, request.Board.GetCell(1, 1).Mark, request.Board.GetCell(2, 0).Mark))
        {
            return Task.FromResult(CheckWinnerResult.Found(request.Board.GetCell(0, 2).Mark));
        }

        return Task.FromResult(CheckWinnerResult.None());
    }

    private static bool AllMatch(PlayerMark a, PlayerMark b, PlayerMark c)
        => a != PlayerMark.None && a == b && b == c;
}
