using FastEndpoints;
using GameStateService.Services;
using Microsoft.AspNetCore.Http;
using GameStateWorkflowService = GameStateService.Services.GameStateService;

namespace GameStateService.Endpoints.Games.Create;

public class CreateGameEndpoint : EndpointWithoutRequest<CreateGameResponse>
{
    private readonly GameStateWorkflowService _gameStateService;

    public CreateGameEndpoint(GameStateWorkflowService gameStateService)
    {
        _gameStateService = gameStateService;
    }

    public override void Configure()
    {
        Post("/api/games");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var game = await _gameStateService.CreateGameAsync(ct);

        HttpContext.Response.StatusCode = 202;
        await HttpContext.Response.WriteAsJsonAsync(new CreateGameResponse { GameId = game.GameId }, ct);
    }
}
