namespace Service.Contracts.Requests;

public record CreateGameRequest
{
    public required Guid PlayerId { get; init; }
    public required string PlayerName { get; init; }
}