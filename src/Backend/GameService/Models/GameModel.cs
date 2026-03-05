namespace GameService.Models;

public class GameModel
{
    public required string Id { get; init; }
    public GameStatus Status { get; set; } = GameStatus.Created;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public required PlayerModel Player1 { get; init; }
    public PlayerModel? Player2 { get; set; }
}
