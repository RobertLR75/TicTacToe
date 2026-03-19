using NSubstitute;
using UserService.Features.Users.Endpoints.Get;
using UserService.Features.Users.Endpoints.List;
using UserService.Features.Users.Entities;
using Xunit;

namespace UserService.UnitTests;

public sealed class GetAndListUserHandlerUnitTests : UserServiceUnitTestBase
{
    [Fact]
    public async Task GetUser_reads_from_cache_before_storage()
    {
        var storage = CreateStorage();
        var cache = CreateCache();
        var user = CreateUser();
        cache.GetUserAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);
        var sut = new GetUserHandler(storage, cache);

        var result = await sut.HandleAsync(new GetUserQuery(user.Id), CancellationToken.None);

        Assert.Equal(user.Id, result.Id);
        await storage.DidNotReceive().GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ListUsers_reads_from_storage_and_populates_cache_when_cache_empty()
    {
        var storage = CreateStorage();
        var cache = CreateCache();
        var users = new[] { CreateUser("Alice"), CreateUser("Bob") };
        cache.GetUsersAsync(Arg.Any<CancellationToken>()).Returns((IReadOnlyList<UserEntity>?)null);
        storage.ListAsync(Arg.Any<CancellationToken>()).Returns(users);
        var sut = new ListUsersHandler(storage, cache);

        var result = await sut.HandleAsync(new ListUsersQuery(), CancellationToken.None);

        Assert.Equal(2, result.Users.Count);
        await cache.Received(1).SetUsersAsync(users, Arg.Any<CancellationToken>());
    }
}
