using GameService.Services;
using Service.Contracts.Responses;

namespace GameService.Endpoints.Games.Get;

public interface IGetGameHandler : IRequestHandler<GetGameQuery, GetGameQueryResult>;

public sealed record GetGameQuery(Guid GameId) : IRequest<GetGameQueryResult>;

public sealed record GetGameQueryResult
{
    public required bool Found { get; init; }

    public required bool DependencyUnavailable { get; init; }

    public GetGameResponse? Response { get; init; }

    public static GetGameQueryResult Success(GetGameResponse response) => new()
    {
        Found = true,
        DependencyUnavailable = false,
        Response = response
    };

    public static GetGameQueryResult NotFound() => new()
    {
        Found = false,
        DependencyUnavailable = false
    };

    public static GetGameQueryResult Unavailable() => new()
    {
        Found = false,
        DependencyUnavailable = true
    };
}

public sealed class GetGameHandler(IGameStateReadClient gameStateReadClient) : IGetGameHandler
{
    public async Task<GetGameQueryResult> HandleAsync(GetGameQuery request, CancellationToken ct = default)
    {
        var result = await gameStateReadClient.GetGameAsync(request.GameId, ct);

        if (result.Unavailable)
        {
            return GetGameQueryResult.Unavailable();
        }

        if (!result.Found || result.Response is null)
        {
            return GetGameQueryResult.NotFound();
        }

        return GetGameQueryResult.Success(result.Response);
    }
}
