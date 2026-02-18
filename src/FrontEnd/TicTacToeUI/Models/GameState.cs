namespace TicTacToeUI.Models;

public class GameState
{
    public Board Board { get; } = new();
    public PlayerMark CurrentPlayer { get; set; } = PlayerMark.X;
    public PlayerMark Winner { get; set; } = PlayerMark.None;
    public bool IsDraw { get; set; }
    public bool IsOver => Winner != PlayerMark.None || IsDraw;
}
