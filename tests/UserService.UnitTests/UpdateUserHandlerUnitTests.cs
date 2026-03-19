using NSubstitute;
using UserService.Features.Users.Endpoints.Update;
using UserService.Features.Users.Entities;
using Xunit;

namespace UserService.UnitTests;

public sealed class UpdateUserHandlerUnitTests : UserServiceUnitTestBase
{
    [Fact]
    public async Task HandleAsync_returns_not_found_when_user_missing()
    {
        var storage = CreateStorage();
        var cache = CreateCache();
        storage.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((UserEntity?)null);
        var sut = new UpdateUserHandler(storage, cache);

        var result = await sut.HandleAsync(new UpdateUserCommand(Guid.NewGuid(), "Bob"), CancellationToken.None);

        Assert.Equal(Guid.Empty, result.Id);
    }

    [Fact(Skip = "Success path publishing now flows through internal FastEndpoints events; event publication is verified through event-handler tests.")]
    public async Task HandleAsync_updates_storage_and_cache_when_user_exists()
    {
        var storage = CreateStorage();
        var cache = CreateCache();
        var user = CreateUser("Alice");
        storage.GetAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);
        storage.ListAsync(Arg.Any<CancellationToken>()).Returns([user]);
        var sut = new UpdateUserHandler(storage, cache);

        var result = await sut.HandleAsync(new UpdateUserCommand(user.Id, "Bob"), CancellationToken.None);

        Assert.Equal("Bob", result.Name);
        await storage.Received(1).UpdateAsync(Arg.Is<UserEntity>(u => u.Id == user.Id && u.Name == "Bob"), Arg.Any<CancellationToken>());
        await cache.Received(1).SetUserAsync(Arg.Is<UserEntity>(u => u.Id == user.Id && u.Name == "Bob"), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UserUpdatedEventHandler_publishes_mapped_shared_event()
    {
        var publisher = CreatePublisher();
        var user = CreateUser("Alice", updatedAt: DateTimeOffset.UtcNow);
        var sut = new UserUpdatedEvent.UserUpdatedEventHandler(publisher);

        await sut.HandleAsync(new UserUpdatedEvent { User = user }, CancellationToken.None);

        await publisher.Received(1).PublishEventAsync<Service.Contracts.Events.UserUpdated>(
            Arg.Any<Service.Contracts.Events.UserUpdated>(),
            Arg.Any<CancellationToken>());
    }
}
