namespace Service.Contracts.CreateGame;

public record CreateGameRequest
{
    public required Guid PlayerId { get; init; }
    public required string PlayerName { get; init; }
}