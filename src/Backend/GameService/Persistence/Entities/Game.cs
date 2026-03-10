namespace GameService.Persistence.Entities;

public sealed class Game
{
    public required string Id { get; init; }
    public required string Status { get; set; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public required string Player1Id { get; init; }
    public string? Player2Id { get; set; }

    public required Player Player1 { get; init; }
    public Player? Player2 { get; set; }
}
