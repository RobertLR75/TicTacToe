using GameNotificationService.Persistence;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Xunit;

namespace GameNotificationService.UnitTests;

public sealed class NotificationPersistenceInitializerHostedServiceUnitTests
{
    [Fact]
    public async Task StartAsync_stops_after_first_successful_initialization()
    {
        var initializer = Substitute.For<INotificationPersistenceInitializer>();
        initializer.EnsureInitializedAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));

        var sut = new NotificationPersistenceInitializerHostedService(
            initializer,
            NullLogger<NotificationPersistenceInitializerHostedService>.Instance);

        await sut.StartAsync(CancellationToken.None);
        var executeTask = sut.ExecuteTask;
        Assert.NotNull(executeTask);
        await executeTask.WaitAsync(TimeSpan.FromSeconds(1));

        await initializer.Received(1).EnsureInitializedAsync(Arg.Any<CancellationToken>());
        Assert.True(executeTask.IsCompletedSuccessfully);

        await sut.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task StartAsync_keeps_running_after_failed_initialization_until_stopped()
    {
        var initializer = Substitute.For<INotificationPersistenceInitializer>();
        initializer.EnsureInitializedAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(false));

        var sut = new NotificationPersistenceInitializerHostedService(
            initializer,
            NullLogger<NotificationPersistenceInitializerHostedService>.Instance);

        await sut.StartAsync(CancellationToken.None);
        await Task.Delay(100);

        await initializer.Received(1).EnsureInitializedAsync(Arg.Any<CancellationToken>());
        var executeTask = sut.ExecuteTask;
        Assert.NotNull(executeTask);
        Assert.False(executeTask.IsCompleted);

        await sut.StopAsync(CancellationToken.None);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await executeTask.WaitAsync(TimeSpan.FromSeconds(1)));
    }
}


