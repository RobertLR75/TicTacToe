using FastEndpoints;
using GameService.Services;
using Service.Contracts.Requests;
using Service.Contracts.Responses;

namespace GameService.Endpoints.Games.UpdateStatus;

public class UpdateGameStatusEndpoint : Endpoint<UpdateGameStatusRequest, UpdateGameStatusResponse, UpdateGameStatusMapper>
{
    private readonly IUpdateGameStatusHandler _statusUpdateHandler;

    public UpdateGameStatusEndpoint(
       IUpdateGameStatusHandler statusUpdateHandler)
    {
        _statusUpdateHandler = statusUpdateHandler;
    }

    public override void Configure()
    {
        Verbs(Http.PUT);
        Routes("/api/games/{Id}/status");
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

        Response = Map.FromEntity(result);

        await Send.OkAsync(Response, ct);
    }
}
