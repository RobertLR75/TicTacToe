using Service.Contracts.Requests;
using Service.Contracts.Responses;
using SharedLibrary.FastEndpoints;
namespace UserService.Features.Users.Endpoints.UpdateStatus;

public sealed class UpdateUserStatusEndpoint(IUpdateUserStatusHandler handler)
    : BaseCommandEndpoint<UpdateUserStatusRequest, UpdateUserStatusResponse, UpdateUserStatusCommand, UpdateUserStatusResult, UpdateUserStatusMapper>(handler)
{
    public override void Configure()
    {
        Put("/api/users/{Id}/status");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Update user status";
            s.Description = "Updates a user status to Disabled.";
        });
    }

    protected override async Task HandleEntityAsync(UpdateUserStatusResult result, CancellationToken ct)
    {
        if (result.InvalidStatus)
        {
            AddError("Status must be Disabled");
            ThrowIfAnyErrors(400);
            await Send.ErrorsAsync(cancellation: ct);
        }

        if (!result.Found)
        {
            AddError("User not found");
            ThrowIfAnyErrors(404);
            await Send.NotFoundAsync(ct);
        }
    }
}
