using UserService.Features.Users.Entities;

namespace UserService.Services;

public interface IUserCacheService
{
    Task<UserEntity?> GetUserAsync(Guid id, CancellationToken ct = default);
    Task SetUserAsync(UserEntity user, CancellationToken ct = default);
    Task<IReadOnlyList<UserEntity>?> GetUsersAsync(CancellationToken ct = default);
    Task SetUsersAsync(IReadOnlyList<UserEntity> users, CancellationToken ct = default);
}
