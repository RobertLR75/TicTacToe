using Service.Contracts.Requests;
using Service.Contracts.Responses;
using SharedLibrary.FastEndpoints;
using UserService.Features.Users.Entities;
using UserService.Services;

namespace UserService.Features.Users.Endpoints.Update;

public sealed class UpdateUserMapper : BaseCommandMapper<UpdateUserRequest, UserModel, UpdateUserCommand, UserEntity>
{
    public override UpdateUserCommand ToCommand(UpdateUserRequest req) => new(req.Id, req.Name);

    public override UserModel FromEntity(UserEntity user) => user.ToResponse();

    public override Task<UserModel> FromEntityAsync(UserEntity user, CancellationToken ct)
        => Task.FromResult(FromEntity(user));
}
