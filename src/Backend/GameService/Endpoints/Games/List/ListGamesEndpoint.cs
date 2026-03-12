using FastEndpoints;
using GameService.Models;
using GameService.Services;
using Service.Contracts.Requests;
using Service.Contracts.Responses;

namespace GameService.Endpoints.Games.List;

public class ListGamesEndpoint : Endpoint<ListGamesRequest, ListGamesResponse, ListGamesMapper>
{
    private readonly IRequestHandler<ListGamesQuery, IEnumerable<Game>> _handler;

    public ListGamesEndpoint(IRequestHandler<ListGamesQuery, IEnumerable<Game>> handler)
    {
        _handler = handler;
    }
    
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

    public override async Task HandleAsync(ListGamesRequest request, CancellationToken ct)
    {

        var query = Map.ToEntity(request);
        var games = await _handler.HandleAsync(query, ct);

        Response = Map.FromEntity(games);

        await Send.OkAsync(Response, ct);
    }
}
