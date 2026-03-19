using GameNotificationService.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Service.Contracts.Events;
using Xunit;

namespace GameNotificationService.UnitTests;

public abstract class GameNotificationServiceUnitTestBase : IAsyncLifetime
{
    protected GameNotificationServiceUnitTestFixture Fixture { get; } = new();

    public virtual Task InitializeAsync() => Fixture.InitializeAsync();

    public virtual Task DisposeAsync() => Fixture.DisposeAsync();

    protected IOptions<MessagingOptions> CreateMessagingOptions(bool enableEventConsumers = true)
        => Fixture.CreateMessagingOptions(enableEventConsumers);

    protected IConfiguration BuildConfiguration(Dictionary<string, string?> values)
        => Fixture.BuildConfiguration(values);

    protected GameStateInitialized BuildInitializedEvent(string? eventId = null, string gameId = "11111111-1111-1111-1111-111111111111")
        => Fixture.BuildInitializedEvent(eventId, gameId);

    protected GameStateUpdated BuildUpdatedEvent(string? eventId = null, string gameId = "11111111-1111-1111-1111-111111111111")
        => Fixture.BuildUpdatedEvent(eventId, gameId);

    protected GameNotificationServiceUnitTestFixture.InMemoryNotificationRepository CreateRepository(bool rejectDuplicates = false)
        => Fixture.CreateRepository(rejectDuplicates);

    protected GameNotificationServiceUnitTestFixture.RecordingNotificationPublisher CreatePublisher()
        => Fixture.CreatePublisher();
}
