using Service.Contracts.Requests;
using Service.Contracts.Responses;
using SharedLibrary.FastEndpoints;
using UserService.Features.Users.Entities;
using UserStatusEnum = Service.Contracts.Shared.UserStatusEnum;

namespace UserService.Features.Users.Endpoints.UpdateStatus;

public sealed class UpdateUserStatusMapper : BaseCommandMapper<UpdateUserStatusRequest, UpdateUserStatusResponse, UpdateUserStatusCommand, UpdateUserStatusResult>
{
    public override UpdateUserStatusCommand ToCommand(UpdateUserStatusRequest req)
        => new(req.Id, (UserStatus)req.Status);

    public override UpdateUserStatusResponse FromEntity(UpdateUserStatusResult result)
    {
        ArgumentNullException.ThrowIfNull(result.User);

        var user = result.User;

        return new()
        {
            Id = user.Id.ToString("D"),
            Status = (UserStatusEnum)user.Status
        };
    }

    public override Task<UpdateUserStatusResponse> FromEntityAsync(UpdateUserStatusResult result, CancellationToken ct)
        => Task.FromResult(FromEntity(result));
}
