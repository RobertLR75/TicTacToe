using FastEndpoints;
using GameStateService.Services;

namespace GameStateService.Endpoints.Games.Create;

public class CreateGameEndpoint : EndpointWithoutRequest<CreateGameResponse, CreateGameMapper>
{
    private readonly IRequestHandler<CreateGame, Models.GameState> _handler;

    public CreateGameEndpoint(IRequestHandler<CreateGame, Models.GameState> handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Post("/api/games");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var game = await _handler.HandleAsync(new CreateGame(), ct);

        Response = Map.FromEntity(game);

        HttpContext.Response.StatusCode = 202;
        await HttpContext.Response.WriteAsJsonAsync(Response, ct);
    }
}
