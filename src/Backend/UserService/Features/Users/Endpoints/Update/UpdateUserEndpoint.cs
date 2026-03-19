using Service.Contracts.Requests;
using Service.Contracts.Responses;
using SharedLibrary.FastEndpoints;
using UserService.Features.Users.Entities;

namespace UserService.Features.Users.Endpoints.Update;

public sealed class UpdateUserEndpoint(IUpdateUserHandler handler)
    : BaseCommandEndpoint<UpdateUserRequest, UserModel, UpdateUserCommand, UserEntity, UpdateUserMapper>(handler)
{
    public override void Configure()
    {
        Put("/api/users/{Id}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Update a user";
            s.Description = "Updates a user and publishes a user updated event.";
        });
    }

    protected override async Task HandleEntityAsync(UserEntity result, CancellationToken ct)
    {
        if (result.Id == Guid.Empty)
        {
            AddError("User not found");
            ThrowIfAnyErrors(404);
            await Send.NotFoundAsync(ct);
        }
    }
}
