namespace GameService.Persistence.Entities;

public sealed class PlayerEntity
{
    public required string Id { get; init; }
    public required string Name { get; set; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; set; }
}
