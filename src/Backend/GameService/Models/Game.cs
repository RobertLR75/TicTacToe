using SharedLibrary.Interfaces;

namespace GameService.Models;

public class Game : IEntity
{
    public required Guid Id { get; set; }
    public GameStatus Status { get; set; } = GameStatus.Created;
    public DateTimeOffset CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
    public required Player Player1 { get; init; }
    public Player? Player2 { get; set; }
}
