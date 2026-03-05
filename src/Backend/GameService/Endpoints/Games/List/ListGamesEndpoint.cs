using FastEndpoints;
using GameService.Models;
using GameService.Services;

namespace GameService.Endpoints.Games.List;

public class ListGamesEndpoint : EndpointWithoutRequest<ListGamesResponse>
{
    private readonly GameRepository _repository;

    public ListGamesEndpoint(GameRepository repository)
    {
        _repository = repository;
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

    public override async Task HandleAsync(CancellationToken ct)
    {
        var games = await _repository.GetGamesByStatusAsync(GameStatus.Created, ct);

        Response = new ListGamesResponse
        {
            Games = games.Select(g => new GameDto
            {
                Id = g.Id,
                Status = g.Status,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt,
                Player1 = new PlayerDto { Id = g.Player1.Id, Name = g.Player1.Name },
                Player2 = g.Player2 is not null
                    ? new PlayerDto { Id = g.Player2.Id, Name = g.Player2.Name }
                    : null
            }).ToList()
        };

        await Send.OkAsync(Response, ct);
    }
}
