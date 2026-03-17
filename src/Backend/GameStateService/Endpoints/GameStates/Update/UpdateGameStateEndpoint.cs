using FastEndpoints;
using GameStateService.Services;
using Service.Contracts.Requests;

namespace GameStateService.Endpoints.GameStates.Update;

public class UpdateGameStateEndpoint : Endpoint<UpdateGameStateRequest>
{
    private readonly IRequestHandler<UpdateGameState, MakeMoveCommandResult> _handler;

    public UpdateGameStateEndpoint(IRequestHandler<UpdateGameState, MakeMoveCommandResult> handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Post("/api/game-states/{GameId}/moves");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateGameStateRequest req, CancellationToken ct)
    {
        var result = await _handler.HandleAsync(new UpdateGameState(req.GameId, req.Row, req.Col), ct);

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

        HttpContext.Response.StatusCode = StatusCodes.Status202Accepted;
        await HttpContext.Response.CompleteAsync();
    }
}
