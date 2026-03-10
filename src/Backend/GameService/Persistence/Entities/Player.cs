namespace GameService.Persistence.Entities;

public sealed class Player
{
    public required string Id { get; init; }
    public required string Name { get; set; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
