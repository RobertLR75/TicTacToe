using GameService.Features.Games.Entities;
using Service.Contracts.Requests;
using Service.Contracts.Responses;
using SharedLibrary.FastEndpoints;

namespace GameService.Features.Games.Endpoints.Get;

public sealed class GetGameEndpoint(IGetGameHandler handler)
    : BaseQueryEndpoint<GetGameRequest, GetGameResponse, GetGameQuery, GameEntity, GetGameMapper>(handler)
{
    public override void Configure()
    {
        Get("/api/games/{GameId}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get a single game by id";
            s.Description = "Returns the current game state through the GameService public API.";
        });
    }
}
