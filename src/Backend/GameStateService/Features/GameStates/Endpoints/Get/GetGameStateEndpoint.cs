using GameStateService.Features.GameStates.Entities;
using Service.Contracts.Requests;
using Service.Contracts.Responses;
using SharedLibrary.FastEndpoints;

namespace GameStateService.Features.GameStates.Endpoints.Get;

public sealed class GetGameStateEndpoint(IGetGameHandler handler)
    : BaseQueryEndpoint<GetGameRequest, GetGameStateResponse, GetGameQuery, GameEntity, GetGameStateMapper>(handler)
{
    public override void Configure()
    {
        Get("/api/game-states/{GameId}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get a single game state by id";
            s.Description = "Returns the current board, turn, and outcome for a tracked game.";
        });
    }
}
