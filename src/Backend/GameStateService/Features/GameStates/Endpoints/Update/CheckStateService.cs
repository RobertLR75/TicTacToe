using GameStateService.Features.GameStates.Entities;

namespace GameStateService.Features.GameStates.Endpoints.Update;

public interface ICheckStateService
{
    CheckDrawResult CheckDraw(Board board);
    CheckWinnerResult CheckWinner(Board board);
}

public class CheckStateService : ICheckStateService
{
    public CheckDrawResult CheckDraw(Board board)
    {
        for (var row = 0; row < 3; row++)
        {
            for (var col = 0; col < 3; col++)
            {
                if (board.GetCell(row, col).Mark == PlayerMark.None)
                {
                    return CheckDrawResult.False();
                }
            }
        }

        return CheckDrawResult.True();   
    }

    public CheckWinnerResult CheckWinner(Board board)
    {
        for (var i = 0; i < 3; i++)
        {
            if (AllMatch(board.GetCell(i, 0).Mark, board.GetCell(i, 1).Mark, board.GetCell(i, 2).Mark))
            {
                return CheckWinnerResult.Found(board.GetCell(i, 0).Mark);
            }

            if (AllMatch(board.GetCell(0, i).Mark, board.GetCell(1, i).Mark, board.GetCell(2, i).Mark))
            {
                return CheckWinnerResult.Found(board.GetCell(0, i).Mark);
            }
        }

        if (AllMatch(board.GetCell(0, 0).Mark, board.GetCell(1, 1).Mark, board.GetCell(2, 2).Mark))
        {
            return CheckWinnerResult.Found(board.GetCell(0, 0).Mark);
        }

        if (AllMatch(board.GetCell(0, 2).Mark, board.GetCell(1, 1).Mark, board.GetCell(2, 0).Mark))
        {
            return CheckWinnerResult.Found(board.GetCell(0, 2).Mark);
        }

        return CheckWinnerResult.None();
    }


    
    private static bool AllMatch(PlayerMark a, PlayerMark b, PlayerMark c)
        => a != PlayerMark.None && a == b && b == c;
}

public sealed record CheckWinnerResult
{
    public required PlayerMark Winner { get; init; }

    public static CheckWinnerResult None() => new() { Winner = PlayerMark.None };

    public static CheckWinnerResult Found(PlayerMark winner) => new() { Winner = winner };
}

public sealed record CheckDrawResult
{
    public required bool IsDraw { get; init; }

    public static CheckDrawResult True() => new() { IsDraw = true };

    public static CheckDrawResult False() => new() { IsDraw = false };
}