using FastEndpoints;
using GameStateService.Services;
using Microsoft.AspNetCore.Http;

namespace GameStateService.Endpoints.Games.Create;

public class CreateGameEndpoint : EndpointWithoutRequest<CreateGameResponse>
{
    private readonly IRequestHandler<CreateGameCommand, CreateGameResponse> _handler;

    public CreateGameEndpoint(IRequestHandler<CreateGameCommand, CreateGameResponse> handler)
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
        var response = await _handler.HandleAsync(new CreateGameCommand(), ct);

        HttpContext.Response.StatusCode = 202;
        await HttpContext.Response.WriteAsJsonAsync(response, ct);
    }
}
