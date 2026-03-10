using FastEndpoints;
using GameStateService.Services;

namespace GameStateService.Endpoints.Games.Get;

public class GetGameEndpoint : Endpoint<GetGameRequest, GetGameResponse>
{
    private readonly IRequestHandler<GetGame, GetGameQueryResult> _handler;

    public GetGameEndpoint(IRequestHandler<GetGame, GetGameQueryResult> handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Get("/api/games/{GameId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetGameRequest req, CancellationToken ct)
    {
        var result = await _handler.HandleAsync(new GetGame(req.GameId), ct);

        if (!result.Found || result.Response is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        Response = result.Response;

        await Send.OkAsync(Response, ct);
    }
}
