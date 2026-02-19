using FastEndpoints;
using GameService.Services;

namespace GameService.Endpoints.Games.MakeMove;

public class MakeMoveEndpoint : Endpoint<MakeMoveRequest, MakeMoveResponse>
{
    private readonly GameRepository _repository;
    private readonly GameLogicService _logicService;

    public MakeMoveEndpoint(GameRepository repository, GameLogicService logicService)
    {
        _repository = repository;
        _logicService = logicService;
    }

    public override void Configure()
    {
        Post("/api/games/{GameId}/moves");
        AllowAnonymous();
    }

    public override async Task HandleAsync(MakeMoveRequest req, CancellationToken ct)
    {
        var game = _repository.GetGame(req.GameId);
        
        if (game is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (game.IsOver)
        {
            AddError("Game is already over");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        if (!game.Board.IsEmpty(req.Row, req.Col))
        {
            AddError("Cell is already occupied");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        _logicService.MakeMove(game, req.Row, req.Col);
        _repository.UpdateGame(game);

        Response = new MakeMoveResponse
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

