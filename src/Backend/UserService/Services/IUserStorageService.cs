using UserService.Features.Users.Entities;

namespace UserService.Services;

public interface IUserStorageService
{
    Task<UserEntity> CreateAsync(UserEntity user, CancellationToken ct = default);
    Task UpdateAsync(UserEntity user, CancellationToken ct = default);
    Task<UserEntity?> GetAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<UserEntity>> ListAsync(CancellationToken ct = default);
}
