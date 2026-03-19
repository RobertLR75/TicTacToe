using NSubstitute;
using UserService.Features.Users.Entities;
using UserService.Services;

namespace UserService.UnitTests;

public sealed class UserServiceUnitTestFixture
{
    public UserEntity CreateUser(string name = "Alice", Guid? id = null, DateTimeOffset? createdAt = null, DateTimeOffset? updatedAt = null, UserStatus status = UserStatus.Active)
    {
        return new UserEntity
        {
            Id = id ?? Guid.NewGuid(),
            Name = name,
            Status = status,
            CreatedAt = createdAt ?? DateTimeOffset.UtcNow,
            UpdatedAt = updatedAt
        };
    }

    public IUserStorageService CreateStorage() => Substitute.For<IUserStorageService>();

    public IUserCacheService CreateCache() => Substitute.For<IUserCacheService>();

    public IUserEventPublisher CreatePublisher() => Substitute.For<IUserEventPublisher>();
}
