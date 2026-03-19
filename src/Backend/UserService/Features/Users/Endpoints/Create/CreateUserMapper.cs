using Service.Contracts.Requests;
using Service.Contracts.Responses;
using SharedLibrary.FastEndpoints;
using UserService.Features.Users.Entities;
using UserService.Services;

namespace UserService.Features.Users.Endpoints.Create;

public sealed class CreateUserMapper : BaseCommandMapper<CreateUserRequest, UserModel, CreateUserCommand, UserEntity>
{
    public override CreateUserCommand ToCommand(CreateUserRequest req) => new(req.Name);

    public override UserModel FromEntity(UserEntity user) => user.ToResponse();

    public override Task<UserModel> FromEntityAsync(UserEntity user, CancellationToken ct)
        => Task.FromResult(FromEntity(user));
}
