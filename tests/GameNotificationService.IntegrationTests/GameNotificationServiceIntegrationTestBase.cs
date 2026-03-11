using GameNotificationService.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace GameNotificationService.IntegrationTests;

public abstract class GameNotificationServiceIntegrationTestBase
{
    protected GameNotificationServiceIntegrationTestBase(GameNotificationServiceIntegrationTestFixture fixture)
    {
        Fixture = fixture;
    }

    protected GameNotificationServiceIntegrationTestFixture Fixture { get; }

    protected ServiceProvider CreateServiceProvider(bool enableEventConsumers = false, Action<IServiceCollection>? configureServices = null)
        => Fixture.CreateServiceProvider(enableEventConsumers, configureServices);

    protected GameNotificationServiceWebApplicationFactory CreateFactory(Dictionary<string, string?>? overrides = null)
        => new(Fixture.BuildConfigurationValues(enableEventConsumers: false, overrides));

    protected Task ResetDatabaseAsync(IServiceProvider provider)
        => Fixture.ResetDatabaseAsync(provider);

    protected static async Task PersistNotificationAsync(IServiceProvider services, NotificationWriteModel notification)
    {
        using var scope = services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
        await repository.TryAddAsync(notification);
    }

    protected static async Task<IReadOnlyList<NotificationRecord>> ListNotificationsAsync(IServiceProvider services, NotificationQuery query)
    {
        using var scope = services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
        return await repository.ListAsync(query);
    }

    protected static NotificationWriteModel CreateNotification(
        string eventId,
        string gameId,
        string eventType,
        DateTimeOffset occurredAtUtc,
        DateTimeOffset? receivedAtUtc = null,
        string? payloadSummary = null)
        => new()
        {
            EventId = eventId,
            GameId = gameId,
            EventType = eventType,
            PayloadSummary = payloadSummary ?? "{}",
            OccurredAtUtc = occurredAtUtc,
            ReceivedAtUtc = receivedAtUtc ?? occurredAtUtc.AddSeconds(1)
        };
}
