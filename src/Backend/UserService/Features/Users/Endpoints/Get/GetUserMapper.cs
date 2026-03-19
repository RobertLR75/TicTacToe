using Service.Contracts.Requests;
using Service.Contracts.Responses;
using SharedLibrary.FastEndpoints;
using UserService.Features.Users.Entities;
using UserService.Services;

namespace UserService.Features.Users.Endpoints.Get;

public sealed class GetUserMapper : BaseQueryMapper<GetUserRequest, UserModel, GetUserQuery, UserEntity>
{
    public override GetUserQuery ToQuery(GetUserRequest req) => new(req.Id);

    public override UserModel FromEntity(UserEntity entity) => entity.ToResponse();

    public override Task<UserModel> FromEntityAsync(UserEntity entity, CancellationToken ct)
        => Task.FromResult(FromEntity(entity));
}
