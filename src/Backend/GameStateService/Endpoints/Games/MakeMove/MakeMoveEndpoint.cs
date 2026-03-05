using FastEndpoints;
using GameStateService.Services;
using GameStateWorkflowService = GameStateService.Services.GameStateService;

namespace GameStateService.Endpoints.Games.MakeMove;

public class MakeMoveEndpoint : Endpoint<MakeMoveRequest>
{
    private readonly GameStateWorkflowService _gameStateService;

    public MakeMoveEndpoint(GameStateWorkflowService gameStateService)
    {
        _gameStateService = gameStateService;
    }

    public override void Configure()
    {
        Post("/api/games/{GameId}/moves");
        AllowAnonymous();
    }

    public override async Task HandleAsync(MakeMoveRequest req, CancellationToken ct)
    {
        var result = await _gameStateService.MakeMoveAsync(req.GameId, req.Row, req.Col, ct);

        if (result.Status == GameMoveStatus.NotFound)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (result.Status == GameMoveStatus.GameOver)
        {
            AddError("Game is already over");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        if (result.Status == GameMoveStatus.CellOccupied)
        {
            AddError("Cell is already occupied");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        if (result.Game is null)
        {
            AddError("Unable to apply move");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        HttpContext.Response.StatusCode = 202;
        await Task.CompletedTask;
    }
}
