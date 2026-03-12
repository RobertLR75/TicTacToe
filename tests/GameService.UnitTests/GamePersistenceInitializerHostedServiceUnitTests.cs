using GameService.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Xunit;

namespace GameService.UnitTests;

public sealed class GamePersistenceInitializerHostedServiceUnitTests
{
    [Fact]
    public async Task EnsureGamePersistenceReadyBeforeStartupAsync_returns_after_first_successful_initialization()
    {
        var services = new ServiceCollection();
        var initializer = Substitute.For<IGamePersistenceInitializer>();
        initializer.EnsureInitializedAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));
        services.AddSingleton(initializer);
        await using var provider = services.BuildServiceProvider();

        await provider.EnsureGamePersistenceReadyBeforeStartupAsync(NullLogger.Instance, CancellationToken.None);

        await initializer.Received(1).EnsureInitializedAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task WaitForGamePersistenceReadyAsync_retries_until_initialization_succeeds()
    {
        var services = new ServiceCollection();
        var initializer = Substitute.For<IGamePersistenceInitializer>();
        initializer.EnsureInitializedAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false), Task.FromResult(false), Task.FromResult(true));
        services.AddSingleton(initializer);
        await using var provider = services.BuildServiceProvider();

        await provider.WaitForGamePersistenceReadyAsync(NullLogger.Instance, TimeSpan.FromMilliseconds(1), CancellationToken.None);

        await initializer.Received(3).EnsureInitializedAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task WaitForGamePersistenceReadyAsync_retries_until_cancellation_when_initialization_keeps_failing()
    {
        var services = new ServiceCollection();
        var initializer = Substitute.For<IGamePersistenceInitializer>();
        initializer.EnsureInitializedAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(false));
        services.AddSingleton(initializer);
        await using var provider = services.BuildServiceProvider();
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await provider.WaitForGamePersistenceReadyAsync(NullLogger.Instance, TimeSpan.FromMilliseconds(10), cts.Token));

        await initializer.Received().EnsureInitializedAsync(Arg.Any<CancellationToken>());
    }
}

