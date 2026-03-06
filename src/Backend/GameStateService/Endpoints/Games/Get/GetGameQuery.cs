using GameStateService.Services;

namespace GameStateService.Endpoints.Games.Get;

public sealed record GetGameQuery(string GameId) : IRequest<GetGameQueryResult>;

public sealed record GetGameQueryResult
{
    public required bool Found { get; init; }
    public GetGameResponse? Response { get; init; }

    public static GetGameQueryResult NotFound() => new() { Found = false };

    public static GetGameQueryResult Success(GetGameResponse response) => new()
    {
        Found = true,
        Response = response
    };
}
