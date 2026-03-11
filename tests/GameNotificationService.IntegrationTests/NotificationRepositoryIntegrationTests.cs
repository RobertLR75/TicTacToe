using GameNotificationService.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GameNotificationService.IntegrationTests;

[Collection(GameNotificationCollection.Name)]
public sealed class NotificationRepositoryIntegrationTests : GameNotificationServiceIntegrationTestBase
{
    public NotificationRepositoryIntegrationTests(GameNotificationServiceIntegrationTestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task TryAddAsync_persists_notification_and_ListAsync_returns_newest_first()
    {
        using var provider = CreateServiceProvider();
        await ResetDatabaseAsync(provider);

        using var scope = provider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

        await repository.TryAddAsync(CreateNotification("evt-1", "game-1", "GameStateInitialized", new DateTimeOffset(2026, 3, 10, 10, 0, 0, TimeSpan.Zero)));
        await repository.TryAddAsync(CreateNotification("evt-2", "game-1", "GameStateUpdated", new DateTimeOffset(2026, 3, 10, 10, 5, 0, TimeSpan.Zero)));

        var notifications = await repository.ListAsync(new NotificationQuery(1, 10, "game-1"));

        Assert.Equal(["evt-2", "evt-1"], notifications.Select(x => x.EventId).ToArray());
    }

    [Fact]
    public async Task TryAddAsync_returns_false_for_duplicate_event_id()
    {
        using var provider = CreateServiceProvider();
        await ResetDatabaseAsync(provider);

        using var scope = provider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
        var notification = CreateNotification("evt-dup", "game-1", "GameStateInitialized", DateTimeOffset.UtcNow);

        var first = await repository.TryAddAsync(notification);
        var second = await repository.TryAddAsync(notification);

        Assert.True(first);
        Assert.False(second);
    }

    [Fact]
    public async Task ListAsync_filters_by_game_id_and_applies_paging()
    {
        using var provider = CreateServiceProvider();
        await ResetDatabaseAsync(provider);

        using var scope = provider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

        await repository.TryAddAsync(CreateNotification("evt-1", "game-1", "GameStateInitialized", new DateTimeOffset(2026, 3, 10, 10, 0, 0, TimeSpan.Zero)));
        await repository.TryAddAsync(CreateNotification("evt-2", "game-1", "GameStateUpdated", new DateTimeOffset(2026, 3, 10, 10, 5, 0, TimeSpan.Zero)));
        await repository.TryAddAsync(CreateNotification("evt-3", "game-2", "GameStateUpdated", new DateTimeOffset(2026, 3, 10, 10, 10, 0, TimeSpan.Zero)));

        var notifications = await repository.ListAsync(new NotificationQuery(2, 1, "game-1"));

        var notification = Assert.Single(notifications);
        Assert.Equal("evt-1", notification.EventId);
        Assert.Equal("game-1", notification.GameId);
    }
}
