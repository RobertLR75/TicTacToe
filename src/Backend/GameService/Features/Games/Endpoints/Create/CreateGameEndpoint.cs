using GameService.Features.Games.Endpoints.Get;
using GameService.Features.Games.Entities;
using Service.Contracts.Requests;
using Service.Contracts.Responses;
using SharedLibrary.FastEndpoints;

namespace GameService.Features.Games.Endpoints.Create;

public sealed class CreateGameEndpoint(ICreateGameHandler handler)
    : BaseCommandEndpoint<CreateGameRequest, CreateGameResponse, CreateGameCommand, GameEntity, CreateGameMapper>(handler)
{

    public override void Configure()
    {
        Post("/api/games");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Create a new game";
            s.Description = "Creates a new game with status Created and the requesting player as Player1";
        });
    }

    
    protected override Task HandleResponseAsync(CreateGameResponse response, CancellationToken ct)
        => Send.CreatedAtAsync<GetGameEndpoint>(new { GameId = response.Id }, Response, cancellation: ct);
}
