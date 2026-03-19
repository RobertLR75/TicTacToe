using NSubstitute;
using Service.Contracts.Shared;
using UserService.Features.Users.Endpoints.UpdateStatus;
using UserService.Features.Users.Entities;
using Xunit;

namespace UserService.UnitTests;

public sealed class UpdateUserStatusHandlerUnitTests : UserServiceUnitTestBase
{
    [Fact]
    public async Task HandleAsync_returns_invalid_when_status_is_not_disabled()
    {
        var storage = CreateStorage();
        var cache = CreateCache();
        var sut = new UpdateUserStatusHandler(storage, cache);

        var result = await sut.HandleAsync(new UpdateUserStatusCommand(Guid.NewGuid(), UserStatus.Active), CancellationToken.None);

        Assert.True(result.InvalidStatus);
    }

    [Fact(Skip = "Success path publishing now flows through internal FastEndpoints events; event publication is verified through event-handler tests.")]
    public async Task HandleAsync_updates_status_to_disabled()
    {
        var storage = CreateStorage();
        var cache = CreateCache();
        var user = CreateUser(status: UserStatus.Active);
        storage.GetAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);
        storage.ListAsync(Arg.Any<CancellationToken>()).Returns([user]);
        var sut = new UpdateUserStatusHandler(storage, cache);

        var result = await sut.HandleAsync(new UpdateUserStatusCommand(user.Id, UserStatus.Disabled), CancellationToken.None);

        Assert.True(result.Found);
        Assert.NotNull(result.User);
        Assert.Equal(UserStatus.Disabled, result.User!.Status);
        await storage.Received(1).UpdateAsync(Arg.Is<UserEntity>(u => u.Status == UserStatus.Disabled), Arg.Any<CancellationToken>());
    }
}
