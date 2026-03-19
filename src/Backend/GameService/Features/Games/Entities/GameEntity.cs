using SharedLibrary.Interfaces;

namespace GameService.Features.Games.Entities;

public class GameEntity : IEntity
{
    public required Guid Id { get; set; }
    public GameStatus Status { get; set; } = GameStatus.Created;
    public DateTimeOffset CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
    public required PlayerEntity Player1 { get; init; }
    public PlayerEntity? Player2 { get; set; }
}


