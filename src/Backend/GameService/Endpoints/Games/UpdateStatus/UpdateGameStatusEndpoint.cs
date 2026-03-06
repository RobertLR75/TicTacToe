using FastEndpoints;
using GameService.Contracts;
using GameService.Services;
using GameService.Models;

namespace GameService.Endpoints.Games.UpdateStatus;

public class UpdateGameStatusEndpoint : Endpoint<UpdateGameStatusRequest, UpdateGameStatusResponse, UpdateGameStatusMapper>
{
    private readonly IRequestHandler<UpdateGameStatusCommand, GameStatusUpdateResult> _statusUpdateHandler;

    public UpdateGameStatusEndpoint(IRequestHandler<UpdateGameStatusCommand, GameStatusUpdateResult> statusUpdateHandler)
    {
        _statusUpdateHandler = statusUpdateHandler;
    }

    public override void Configure()
    {
        Verbs(Http.PUT);
        Routes("/api/game-lobby/{Id}/status");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Update game status";
            s.Description = "Updates game status to Active or Completed using one endpoint";
        });
    }

    public override async Task HandleAsync(UpdateGameStatusRequest req, CancellationToken ct)
    {
        var command = Map.ToEntity(req);
        var result = await _statusUpdateHandler.HandleAsync(command, ct);

        if (result.InvalidStatus)
        {
            AddError("Status must be Active or Completed");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        if (result.NotFound)
        {
            await Send.NotFoundAsync(cancellation: ct);
            return;
        }

        Response = new UpdateGameStatusResponse
        {
            Id = result.Id,
            Status = result.Status!.Value.ToString(),
            UpdatedAt = result.UpdatedAt!.Value
        };

        await Send.OkAsync(Response, ct);
    }
}
