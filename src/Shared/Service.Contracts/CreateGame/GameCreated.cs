namespace Service.Contracts.CreateGame;

public record GameCreated
{
    public Guid GameId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public required string Player1Id { get; init; }
}