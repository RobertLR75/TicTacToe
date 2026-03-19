using Service.Contracts.Requests;
using Service.Contracts.Responses;
using SharedLibrary.FastEndpoints;
using UserService.Features.Users.Entities;
using UserService.Services;

namespace UserService.Features.Users.Endpoints.List;

public sealed class ListUsersMapper : BaseQueryMapper<ListUsersRequest, ListUsersResponse, ListUsersQuery, UserListEntity>
{
    public override ListUsersQuery ToQuery(ListUsersRequest req) => new();

    public override ListUsersResponse FromEntity(UserListEntity entity) => entity.ToListResponse();

    public override Task<ListUsersResponse> FromEntityAsync(UserListEntity entity, CancellationToken ct)
        => Task.FromResult(FromEntity(entity));
}
