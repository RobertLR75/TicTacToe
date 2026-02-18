using TicTacToeUI.Models;

namespace TicTacToeUI.Services;

public class GameService
{
    public GameState CurrentState { get; private set; } = new();

    public void StartNewGame()
    {
        CurrentState = new GameState();
    }

    public void MakeMove(int row, int col)
    {
        if (CurrentState.IsOver) return;
        if (!CurrentState.Board.IsEmpty(row, col)) return;

        CurrentState.Board.SetCell(row, col, CurrentState.CurrentPlayer);

        var winner = CheckWinner();
        if (winner != PlayerMark.None)
        {
            CurrentState.Winner = winner;
        }
        else if (CheckDraw())
        {
            CurrentState.IsDraw = true;
        }
        else
        {
            CurrentState.CurrentPlayer = CurrentState.CurrentPlayer == PlayerMark.X
                ? PlayerMark.O
                : PlayerMark.X;
        }
    }

    private PlayerMark CheckWinner()
    {
        var b = CurrentState.Board;

        for (int i = 0; i < 3; i++)
        {
            if (AllMatch(b.GetCell(i, 0).Mark, b.GetCell(i, 1).Mark, b.GetCell(i, 2).Mark))
                return b.GetCell(i, 0).Mark;

            if (AllMatch(b.GetCell(0, i).Mark, b.GetCell(1, i).Mark, b.GetCell(2, i).Mark))
                return b.GetCell(0, i).Mark;
        }

        if (AllMatch(b.GetCell(0, 0).Mark, b.GetCell(1, 1).Mark, b.GetCell(2, 2).Mark))
            return b.GetCell(0, 0).Mark;

        if (AllMatch(b.GetCell(0, 2).Mark, b.GetCell(1, 1).Mark, b.GetCell(2, 0).Mark))
            return b.GetCell(0, 2).Mark;

        return PlayerMark.None;
    }

    private bool CheckDraw()
    {
        var b = CurrentState.Board;
        for (int r = 0; r < 3; r++)
            for (int c = 0; c < 3; c++)
                if (b.GetCell(r, c).Mark == PlayerMark.None) return false;
        return true;
    }

    private static bool AllMatch(PlayerMark a, PlayerMark b, PlayerMark c)
        => a != PlayerMark.None && a == b && b == c;
}
