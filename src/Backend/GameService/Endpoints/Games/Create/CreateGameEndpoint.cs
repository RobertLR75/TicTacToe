using FastEndpoints;
using GameService.Services;

namespace GameService.Endpoints.Games.Create;

public class CreateGameEndpoint : EndpointWithoutRequest<CreateGameResponse>
{
    private readonly GameRepository _repository;

    public CreateGameEndpoint(GameRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Post("/api/games");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var game = _repository.CreateGame();

        Response = new CreateGameResponse
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

