using Service.Contracts.Requests;
using Service.Contracts.Responses;
using SharedLibrary.FastEndpoints;

namespace GameService.Features.Games.Endpoints.UpdateStatus;

public sealed class UpdateGameStatusEndpoint(IUpdateGameStatusHandler handler)
    : BaseCommandEndpoint<UpdateGameStatusRequest, UpdateGameStatusResponse, UpdateGameStatusCommand, GameStatusUpdateResult, UpdateGameStatusMapper>(handler)
{
    public override void Configure()
    {
        Put("/api/games/{Id}/status");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Update game status";
            s.Description = "Updates game status to Active or Completed using one endpoint";
        });
    }

    protected override async Task HandleEntityAsync(GameStatusUpdateResult result, CancellationToken ct)
    {
        if (result.InvalidStatus)
        {
            AddError("Status must be Active or Completed");
            ThrowIfAnyErrors(400);
            await Send.ErrorsAsync(cancellation: ct);
        }

        if (result.NotFound)
        {
            AddError("Game not found");
            ThrowIfAnyErrors(404);
            await Send.NotFoundAsync(ct);
        }
    }
}