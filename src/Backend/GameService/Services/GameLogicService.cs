using GameService.Models;

namespace GameService.Services;

public class GameLogicService
{
    public void MakeMove(GameState gameState, int row, int col)
    {
        if (gameState.IsOver) return;
        if (!gameState.Board.IsEmpty(row, col)) return;

        gameState.Board.SetCell(row, col, gameState.CurrentPlayer);

        var winner = CheckWinner(gameState.Board);
        if (winner != PlayerMark.None)
        {
            gameState.Winner = winner;
        }
        else if (CheckDraw(gameState.Board))
        {
            gameState.IsDraw = true;
        }
        else
        {
            gameState.CurrentPlayer = gameState.CurrentPlayer == PlayerMark.X
                ? PlayerMark.O
                : PlayerMark.X;
        }
    }

    private PlayerMark CheckWinner(Board board)
    {
        // Check rows and columns
        for (int i = 0; i < 3; i++)
        {
            if (AllMatch(board.GetCell(i, 0).Mark, board.GetCell(i, 1).Mark, board.GetCell(i, 2).Mark))
                return board.GetCell(i, 0).Mark;

            if (AllMatch(board.GetCell(0, i).Mark, board.GetCell(1, i).Mark, board.GetCell(2, i).Mark))
                return board.GetCell(0, i).Mark;
        }

        // Check diagonals
        if (AllMatch(board.GetCell(0, 0).Mark, board.GetCell(1, 1).Mark, board.GetCell(2, 2).Mark))
            return board.GetCell(0, 0).Mark;

        if (AllMatch(board.GetCell(0, 2).Mark, board.GetCell(1, 1).Mark, board.GetCell(2, 0).Mark))
            return board.GetCell(0, 2).Mark;

        return PlayerMark.None;
    }

    private bool CheckDraw(Board board)
    {
        for (int r = 0; r < 3; r++)
            for (int c = 0; c < 3; c++)
                if (board.GetCell(r, c).Mark == PlayerMark.None)
                    return false;
        return true;
    }

    private static bool AllMatch(PlayerMark a, PlayerMark b, PlayerMark c)
        => a != PlayerMark.None && a == b && b == c;
}

