using FastEndpoints;
using GameStateService.Services;

namespace GameStateService.Endpoints.Games.MakeMove;

public class MakeMoveEndpoint : Endpoint<MakeMoveRequest>
{
    private readonly IRequestHandler<MakeMoveCommand, MakeMoveCommandResult> _handler;

    public MakeMoveEndpoint(IRequestHandler<MakeMoveCommand, MakeMoveCommandResult> handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Post("/api/games/{GameId}/moves");
        AllowAnonymous();
    }

    public override async Task HandleAsync(MakeMoveRequest req, CancellationToken ct)
    {
        var result = await _handler.HandleAsync(new MakeMoveCommand(req.GameId, req.Row, req.Col), ct);

        if (result.Status == MakeMoveCommandStatus.NotFound)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (result.Status == MakeMoveCommandStatus.GameOver)
        {
            AddError("Game is already over");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        if (result.Status == MakeMoveCommandStatus.CellOccupied)
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
