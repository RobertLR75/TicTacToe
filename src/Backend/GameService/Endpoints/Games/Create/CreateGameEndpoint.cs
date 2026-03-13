using FastEndpoints;
using GameService.Models;
using GameService.Services;
using Service.Contracts.Requests;
using Service.Contracts.Responses;

namespace GameService.Endpoints.Games.Create;

public class CreateGameEndpoint : Endpoint<CreateGameRequest, CreateGameResponse, CreateGameMapper>
{
    private readonly ICreateGameHandler _handler;

    public CreateGameEndpoint(ICreateGameHandler handler)
    {
        _handler = handler;
    }

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

    public override async Task HandleAsync(CreateGameRequest req, CancellationToken ct)
    {

        var command = Map.ToEntity(req);
        var game = await _handler.HandleAsync(command, ct);


        Response = Map.FromEntity(game);

        await Send.CreatedAtAsync<CreateGameEndpoint>(null, Response, cancellation: ct);
    }
}
