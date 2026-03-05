namespace GameService.Endpoints.Games.Complete;

public record CompleteGameRequest
{
    public required string Id { get; init; }
}

public record CompleteGameResponse
{
    public required string Id { get; init; }
    public required string Status { get; init; }
    public required DateTime UpdatedAt { get; init; }
}
