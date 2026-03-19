using GameService.Features.Games.Entities;
using Service.Contracts.Requests;
using Service.Contracts.Responses;
using SharedLibrary.FastEndpoints;

namespace GameService.Features.Games.Endpoints.List;

public sealed class ListGamesEndpoint(IListGamesHandler handler)
    : BaseQueryEndpoint<ListGamesRequest, ListGamesResponse, ListGamesQuery, List<GameEntity>, ListGamesMapper>(handler)
{
    public override void Configure()
    {
        Get("/api/games");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "List all games with Created status";
            s.Description = "Returns all games that have not been started yet (status = Created)";
        });
    }
}
