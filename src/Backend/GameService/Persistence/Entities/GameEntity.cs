namespace GameService.Persistence.Entities;

public sealed class GameEntity
{
    public required string Id { get; init; }
    public required string Status { get; set; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; set; }
    public required string Player1Id { get; init; }
    public string? Player2Id { get; set; }

    public required PlayerEntity Player1 { get; init; }
    public PlayerEntity? Player2 { get; set; }
}
