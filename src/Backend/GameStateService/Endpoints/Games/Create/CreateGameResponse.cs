namespace GameStateService.Endpoints.Games.Create;

public record CreateGameResponse
{
    public required string GameId { get; init; }
}
