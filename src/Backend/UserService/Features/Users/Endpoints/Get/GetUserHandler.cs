using SharedLibrary.Services.Interfaces;
using UserService.Features.Users.Entities;
using UserService.Services;

namespace UserService.Features.Users.Endpoints.Get;

public interface IGetUserHandler : IRequestHandler<GetUserQuery, UserEntity>;

public sealed record GetUserQuery(Guid Id) : IRequest<UserEntity>, IRequest<Service.Contracts.Responses.UserModel>;

public sealed class GetUserHandler(
    IUserStorageService userStorage,
    IUserCacheService userCache) : IGetUserHandler
{
    public async Task<UserEntity> HandleAsync(GetUserQuery request, CancellationToken ct = default)
    {
        var cached = await userCache.GetUserAsync(request.Id, ct);
        if (cached is not null)
        {
            return cached;
        }

        var user = await userStorage.GetAsync(request.Id, ct);
        if (user is null)
        {
            throw new KeyNotFoundException($"User '{request.Id}' was not found.");
        }

        await userCache.SetUserAsync(user, ct);
        return user;
    }
}
