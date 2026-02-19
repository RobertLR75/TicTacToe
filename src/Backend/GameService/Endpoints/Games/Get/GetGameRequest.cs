using FastEndpoints;

namespace GameService.Endpoints.Games.Get;

public class GetGameRequest
{
    public required string GameId { get; init; }
}

