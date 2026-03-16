using FastEndpoints;
using Service.Contracts.Requests;
using Service.Contracts.Responses;

namespace GameService.Endpoints.Games.Get;

public sealed class GetGameEndpoint(IGetGameHandler handler) : Endpoint<GetGameRequest, GetGameResponse>
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

    public override async Task HandleAsync(GetGameRequest request, CancellationToken ct)
    {
        if (!Guid.TryParse(request.GameId, out var gameId))
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var result = await handler.HandleAsync(new GetGameQuery(gameId), ct);

        if (result.DependencyUnavailable)
        {
            await Send.StringAsync(
                "Game state is temporarily unavailable. Please retry once GameStateService recovers.",
                StatusCodes.Status503ServiceUnavailable,
                cancellation: ct);
            return;
        }

        if (!result.Found || result.Response is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        Response = result.Response;
        await Send.OkAsync(Response, ct);
    }
}
