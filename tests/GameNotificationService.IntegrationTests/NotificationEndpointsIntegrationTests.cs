using System.Net;
using System.Net.Http.Json;
using GameNotificationService.Endpoints.Notifications;
using Xunit;

namespace GameNotificationService.IntegrationTests;

[Collection(GameNotificationCollection.Name)]
public sealed class NotificationEndpointsIntegrationTests : GameNotificationServiceIntegrationTestBase
{
    public NotificationEndpointsIntegrationTests(GameNotificationServiceIntegrationTestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task List_notifications_endpoint_returns_notifications_in_repository_order()
    {
        await using var factory = CreateFactory();
        await factory.ResetDatabaseAsync();
        await PersistNotificationAsync(factory.Services, CreateNotification("evt-1", "game-1", "GameStateInitialized", new DateTimeOffset(2026, 3, 10, 10, 0, 0, TimeSpan.Zero)));
        await PersistNotificationAsync(factory.Services, CreateNotification("evt-2", "game-1", "GameStateUpdated", new DateTimeOffset(2026, 3, 10, 10, 5, 0, TimeSpan.Zero)));
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/notifications");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<List<ListNotificationsResponse>>();
        Assert.NotNull(payload);
        Assert.Equal(["evt-2", "evt-1"], payload.Select(x => x.EventId).ToArray());
    }

    [Fact]
    public async Task List_notifications_endpoint_filters_by_game_id()
    {
        await using var factory = CreateFactory();
        await factory.ResetDatabaseAsync();
        await PersistNotificationAsync(factory.Services, CreateNotification("evt-1", "game-1", "GameStateInitialized", DateTimeOffset.UtcNow.AddMinutes(-2)));
        await PersistNotificationAsync(factory.Services, CreateNotification("evt-2", "game-2", "GameStateUpdated", DateTimeOffset.UtcNow.AddMinutes(-1)));
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/notifications?gameId=game-2");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<List<ListNotificationsResponse>>();
        var notification = Assert.Single(payload!);
        Assert.Equal("game-2", notification.GameId);
        Assert.Equal("evt-2", notification.EventId);
    }

    [Fact]
    public async Task List_notifications_endpoint_clamps_invalid_page_and_page_size()
    {
        await using var factory = CreateFactory(new Dictionary<string, string?>
        {
            ["NotificationQuery:DefaultPageSize"] = "1",
            ["NotificationQuery:MaxPageSize"] = "2"
        });
        await factory.ResetDatabaseAsync();
        await PersistNotificationAsync(factory.Services, CreateNotification("evt-1", "game-1", "GameStateInitialized", new DateTimeOffset(2026, 3, 10, 10, 0, 0, TimeSpan.Zero)));
        await PersistNotificationAsync(factory.Services, CreateNotification("evt-2", "game-1", "GameStateUpdated", new DateTimeOffset(2026, 3, 10, 10, 5, 0, TimeSpan.Zero)));
        await PersistNotificationAsync(factory.Services, CreateNotification("evt-3", "game-1", "GameStateUpdated", new DateTimeOffset(2026, 3, 10, 10, 10, 0, TimeSpan.Zero)));
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/notifications?page=0&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<List<ListNotificationsResponse>>();
        Assert.NotNull(payload);
        Assert.Equal(2, payload.Count);
        Assert.Equal(["evt-3", "evt-2"], payload.Select(x => x.EventId).ToArray());
    }
}
