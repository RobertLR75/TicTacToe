using Service.Contracts.Requests;
using Service.Contracts.Responses;
using SharedLibrary.FastEndpoints;

namespace GameStateService.Features.GameStates.Endpoints.Update;

public sealed class UpdateGameStateEndpoint(IUpdateGameStateHandler handler)
    : BaseCommandEndpoint<UpdateGameStateRequest, UpdateGameStateResponse, UpdateGameStateCommand, MakeMoveCommandResult, UpdateGameStateMapper>(handler)
{
    public override void Configure()
    {
        Post("/api/game-states/{GameId}/moves");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Apply a move to a game state";
            s.Description = "Marks a cell, updates turn state, and emits a game-state-updated event when successful.";
        });
    }

    protected override Task HandleEntityAsync(MakeMoveCommandResult result, CancellationToken ct)
    {
        if (result.Status == MakeMoveCommandStatus.NotFound)
        {
            AddError("Game state not found");
            ThrowIfAnyErrors(404);
        }

        if (result.Status == MakeMoveCommandStatus.GameOver)
        {
            AddError("Game is already over");
            ThrowIfAnyErrors(400);
        }

        if (result.Status == MakeMoveCommandStatus.CellOccupied)
        {
            AddError("Cell is already occupied");
            ThrowIfAnyErrors(400);
        }

        if (result.Game is null)
        {
            AddError("Unable to apply move");
            ThrowIfAnyErrors(400);
        }

        return Task.CompletedTask;
    }

    protected override Task HandleResponseAsync(UpdateGameStateResponse response, CancellationToken ct)
    {
        HttpContext.Response.StatusCode = StatusCodes.Status202Accepted;
        return HttpContext.Response.WriteAsJsonAsync(response, cancellationToken: ct);
    }
}
