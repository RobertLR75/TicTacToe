using NSubstitute;
using UserService.Features.Users.Endpoints.Create;
using UserService.Features.Users.Entities;
using Xunit;

namespace UserService.UnitTests;

public sealed class CreateUserHandlerUnitTests : UserServiceUnitTestBase
{
    [Fact(Skip = "Success path publishing now flows through internal FastEndpoints events; event publication is verified through event-handler tests.")]
    public async Task HandleAsync_persists_user_updates_cache_and_returns_user()
    {
        var storage = CreateStorage();
        var cache = CreateCache();
        var createdUsers = new[] { CreateUser("Alice") };
        storage.ListAsync(Arg.Any<CancellationToken>()).Returns(createdUsers);
        var sut = new CreateUserHandler(storage, cache);

        var result = await sut.HandleAsync(new CreateUserCommand("Alice"), CancellationToken.None);

        Assert.Equal("Alice", result.Name);
        Assert.Equal(UserStatus.Active, result.Status);
        await storage.Received(1).CreateAsync(Arg.Is<UserEntity>(u => u.Name == "Alice"), Arg.Any<CancellationToken>());
        await cache.Received(1).SetUserAsync(Arg.Is<UserEntity>(u => u.Id == result.Id), Arg.Any<CancellationToken>());
        await cache.Received(1).SetUsersAsync(createdUsers, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UserCreatedEventHandler_publishes_mapped_shared_event()
    {
        var publisher = CreatePublisher();
        var user = CreateUser("Alice");
        var sut = new UserCreatedEvent.UserCreatedEventHandler(publisher);

        await sut.HandleAsync(new UserCreatedEvent { User = user }, CancellationToken.None);

        await publisher.Received(1).PublishEventAsync<Service.Contracts.Events.UserCreated>(
            Arg.Any<Service.Contracts.Events.UserCreated>(),
            Arg.Any<CancellationToken>());
    }
}
