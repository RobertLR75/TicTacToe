using GameStateService.Models;
using GameStateService.Services;

namespace GameStateService.GameState;

public sealed record CheckWinner(Board Board) : IRequest<CheckWinnerResult>;

public sealed record CheckWinnerResult
{
    public required PlayerMark Winner { get; init; }

    public static CheckWinnerResult None() => new() { Winner = PlayerMark.None };

    public static CheckWinnerResult Found(PlayerMark winner) => new() { Winner = winner };
}

public sealed class CheckWinnerHandler : IRequestHandler<CheckWinner, CheckWinnerResult>
{
    public Task<CheckWinnerResult> HandleAsync(CheckWinner command, CancellationToken ct = default)
    {
        for (int i = 0; i < 3; i++)
        {
            if (AllMatch(command.Board.GetCell(i, 0).Mark, command.Board.GetCell(i, 1).Mark, command.Board.GetCell(i, 2).Mark))
            {
                return Task.FromResult(CheckWinnerResult.Found(command.Board.GetCell(i, 0).Mark));
            }

            if (AllMatch(command.Board.GetCell(0, i).Mark, command.Board.GetCell(1, i).Mark, command.Board.GetCell(2, i).Mark))
            {
                return Task.FromResult(CheckWinnerResult.Found(command.Board.GetCell(0, i).Mark));
            }
        }

        if (AllMatch(command.Board.GetCell(0, 0).Mark, command.Board.GetCell(1, 1).Mark, command.Board.GetCell(2, 2).Mark))
        {
            return Task.FromResult(CheckWinnerResult.Found(command.Board.GetCell(0, 0).Mark));
        }

        if (AllMatch(command.Board.GetCell(0, 2).Mark, command.Board.GetCell(1, 1).Mark, command.Board.GetCell(2, 0).Mark))
        {
            return Task.FromResult(CheckWinnerResult.Found(command.Board.GetCell(0, 2).Mark));
        }

        return Task.FromResult(CheckWinnerResult.None());
    }

    private static bool AllMatch(PlayerMark a, PlayerMark b, PlayerMark c)
        => a != PlayerMark.None && a == b && b == c;
}
