using UserService.Features.Users.Entities;
using UserService.Services;

namespace UserService.UnitTests;

public abstract class UserServiceUnitTestBase
{
    protected UserServiceUnitTestFixture Fixture { get; } = new();

    protected UserEntity CreateUser(string name = "Alice", Guid? id = null, DateTimeOffset? createdAt = null, DateTimeOffset? updatedAt = null, UserStatus status = UserStatus.Active)
        => Fixture.CreateUser(name, id, createdAt, updatedAt, status);

    protected IUserStorageService CreateStorage() => Fixture.CreateStorage();

    protected IUserCacheService CreateCache() => Fixture.CreateCache();

    protected IUserEventPublisher CreatePublisher() => Fixture.CreatePublisher();
}
