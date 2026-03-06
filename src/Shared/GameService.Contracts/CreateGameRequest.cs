namespace GameService.Contracts;

public record CreateGameRequest
{
    public required Guid PlayerId { get; init; }
    public required string PlayerName { get; init; }
}