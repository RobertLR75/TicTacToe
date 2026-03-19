namespace Service.Contracts.Shared;

public record PlayerModel
{
    public required string PlayerId { get; init; }
    public required string Name { get; init; }
}