using FastEndpoints;
using GameService.Models;
using SharedLibrary.PostgreSql.EntityFramework;

namespace GameService.Endpoints.Games.List;

public class ListGamesEndpoint : Endpoint<ListGamesRequest, ListGamesResponse>
{
    
    private readonly IPostgresSqlStorageService<GameModel> _gameStore;

    public ListGamesEndpoint(IPostgresSqlStorageService<GameModel> gameStore)
    {
        _gameStore = gameStore;
    }
    
    public override void Configure()
    {
        Get("/api/game-lobby");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "List all games with Created status";
            s.Description = "Returns all games that have not been started yet (status = Created)";
        });
    }

    public override async Task HandleAsync(ListGamesRequest request, CancellationToken ct)
    {
        var specification = new SearchByStatusSpecification(request.Status, request.Page, request.PageSize);
        var games = await _gameStore.SearchAsync(specification, ct);

        Response = new ListGamesResponse
        {
            Games = games.Select(g => new GameDto
            {
                Id = g.Id,
                Status = g.Status,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt ?? g.CreatedAt,
                Player1 = new PlayerDto { Id = g.Player1.Id, Name = g.Player1.Name },
                Player2 = g.Player2 is not null
                    ? new PlayerDto { Id = g.Player2.Id, Name = g.Player2.Name }
                    : null
            }).ToList()
        };

        await Send.OkAsync(Response, ct);
    }
}
