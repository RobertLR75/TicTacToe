using SharedLibrary.Services.Interfaces;
using UserService.Features.Users.Entities;
using UserService.Services;

namespace UserService.Features.Users.Endpoints.List;

public interface IListUsersHandler : IRequestHandler<ListUsersQuery, UserListEntity>;

public sealed record ListUsersQuery : IRequest<UserListEntity>, IRequest<Service.Contracts.Responses.ListUsersResponse>;

public sealed class ListUsersHandler(
    IUserStorageService userStorage,
    IUserCacheService userCache) : IListUsersHandler
{
    public async Task<UserListEntity> HandleAsync(ListUsersQuery request, CancellationToken ct = default)
    {
        var cached = await userCache.GetUsersAsync(ct);
        if (cached is not null)
        {
            return new UserListEntity
            {
                Id = Guid.Empty,
                Users = cached
            };
        }

        var users = await userStorage.ListAsync(ct);
        await userCache.SetUsersAsync(users, ct);
        return new UserListEntity
        {
            Id = Guid.Empty,
            Users = users
        };
    }
}
