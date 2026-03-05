namespace GameService.Endpoints.Games.Activate;

public record ActivateGameRequest
{
    public required string Id { get; init; }
    public required string PlayerName { get; init; }
}

public record ActivateGameResponse
{
    public required string Id { get; init; }
    public required string Status { get; init; }
    public required DateTime UpdatedAt { get; init; }
}
