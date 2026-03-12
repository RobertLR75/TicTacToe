using FluentMigrator.Runner;
using GameNotificationService.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Xunit;

namespace GameNotificationService.UnitTests;

public sealed class NotificationPersistenceInitializerUnitTests
{
    [Fact]
    public async Task EnsureInitializedAsync_marks_readiness_unavailable_when_migration_fails()
    {
        var services = new ServiceCollection();
        var runner = Substitute.For<IMigrationRunner>();
        runner.When(x => x.MigrateUp()).Do(_ => throw new InvalidOperationException("db unavailable"));
        services.AddScoped(_ => runner);

        using var provider = services.BuildServiceProvider();
        var readinessState = new NotificationPersistenceReadinessState();
        var sut = new NotificationPersistenceInitializer(provider.GetRequiredService<IServiceScopeFactory>(), readinessState, NullLogger<NotificationPersistenceInitializer>.Instance);

        var success = await sut.EnsureInitializedAsync();

        Assert.False(success);
        Assert.False(readinessState.IsReady);
        Assert.Equal("db unavailable", readinessState.LastErrorMessage);
    }

    [Fact]
    public async Task EnsureInitializedAsync_marks_readiness_ready_when_migration_succeeds()
    {
        var services = new ServiceCollection();
        var runner = Substitute.For<IMigrationRunner>();
        services.AddScoped(_ => runner);

        using var provider = services.BuildServiceProvider();
        var readinessState = new NotificationPersistenceReadinessState();
        var sut = new NotificationPersistenceInitializer(provider.GetRequiredService<IServiceScopeFactory>(), readinessState, NullLogger<NotificationPersistenceInitializer>.Instance);

        var success = await sut.EnsureInitializedAsync();

        Assert.True(success);
        Assert.True(readinessState.IsReady);
        Assert.Null(readinessState.LastErrorMessage);
        runner.Received(1).MigrateUp();
    }
}
