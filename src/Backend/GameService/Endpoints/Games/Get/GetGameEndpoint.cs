using FastEndpoints;
using GameService.Services;

namespace GameService.Endpoints.Games.Get;

public class GetGameEndpoint : Endpoint<GetGameRequest, GetGameResponse>
{
    private readonly GameRepository _repository;

    public GetGameEndpoint(GameRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/games/{GameId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetGameRequest req, CancellationToken ct)
    {
        var game = _repository.GetGame(req.GameId);
        
        if (game is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        Response = new GetGameResponse
        {
            GameId = game.GameId,
            CurrentPlayer = game.CurrentPlayer,
            Winner = game.Winner,
            IsDraw = game.IsDraw,
            IsOver = game.IsOver,
            Board = game.Board.GetAllCells()
                .Select(c => new CellDto(c.Row, c.Col, c.Mark))
                .ToList()
        };

        await Send.OkAsync(ct);
    }
}

