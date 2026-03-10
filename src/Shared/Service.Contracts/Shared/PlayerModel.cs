namespace Service.Contracts.Shared;

public record PlayerModel
{
    public required string Id { get; init; }
    public required string Name { get; init; }
}
