using GameNotificationService.Configuration;
using GameNotificationService.Features.Notifications.Consumers;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace GameNotificationService.UnitTests;

public sealed class ConsumerUnitTests : GameNotificationServiceUnitTestBase
{
    [Fact]
    public async Task GameStateInitialized_consumer_maps_publishes_and_persists_notification()
    {
        var repository = CreateRepository();
        var publisher = CreatePublisher();
        var sut = new GameStateInitializedHandler(repository, CreateMessagingOptions(), publisher, NullLogger<GameStateInitializedHandler>.Instance);

        await sut.HandleAsync(new GameStateInitializedCommand(BuildInitializedEvent()), CancellationToken.None);

        var write = Assert.Single(repository.Writes);
        Assert.Equal("GameStateInitialized", write.EventType);
        var notification = Assert.Single(publisher.InitializedNotifications);
        Assert.Equal(write.GameId, notification.Id);
    }

    [Fact]
    public async Task GameStateUpdated_consumer_maps_publishes_and_persists_notification()
    {
        var repository = CreateRepository();
        var publisher = CreatePublisher();
        var sut = new GameStateUpdatedHandler(repository, CreateMessagingOptions(), publisher, NullLogger<GameStateUpdatedHandler>.Instance);

        await sut.HandleAsync(new GameStateUpdatedCommand(BuildUpdatedEvent()), CancellationToken.None);

        var write = Assert.Single(repository.Writes);
        Assert.Equal("GameStateUpdated", write.EventType);
        var notification = Assert.Single(publisher.UpdatedNotifications);
        Assert.Equal(write.GameId, notification.Id);
    }

    [Fact]
    public async Task GameStateUpdated_consumer_respects_idempotency_when_repository_rejects_duplicate()
    {
        var repository = CreateRepository(rejectDuplicates: true);
        var publisher = CreatePublisher();
        var sut = new GameStateUpdatedHandler(repository, CreateMessagingOptions(), publisher, NullLogger<GameStateUpdatedHandler>.Instance);
        var message = BuildUpdatedEvent();

        await sut.HandleAsync(new GameStateUpdatedCommand(message), CancellationToken.None);
        await sut.HandleAsync(new GameStateUpdatedCommand(message), CancellationToken.None);

        Assert.Single(repository.Writes);
        Assert.Equal(2, publisher.UpdatedNotifications.Count);
    }

    [Fact]
    public async Task GameStateInitialized_consumer_skips_processing_when_toggle_is_off()
    {
        var repository = CreateRepository();
        var publisher = CreatePublisher();
        var sut = new GameStateInitializedHandler(repository, CreateMessagingOptions(enableEventConsumers: false), publisher, NullLogger<GameStateInitializedHandler>.Instance);

        await sut.HandleAsync(new GameStateInitializedCommand(BuildInitializedEvent()), CancellationToken.None);

        Assert.Empty(repository.Writes);
        Assert.Empty(publisher.InitializedNotifications);
    }

    [Fact]
    public async Task GameStateInitialized_consumer_skips_invalid_payload_without_persisting()
    {
        var repository = CreateRepository();
        var publisher = CreatePublisher();
        var sut = new GameStateInitializedHandler(repository, CreateMessagingOptions(), publisher, NullLogger<GameStateInitializedHandler>.Instance);
        var invalidMessage = BuildInitializedEvent() with { EventId = string.Empty };

        await sut.HandleAsync(new GameStateInitializedCommand(invalidMessage), CancellationToken.None);

        Assert.Empty(repository.Writes);
        Assert.Empty(publisher.InitializedNotifications);
    }

    [Fact]
    public async Task GameStateUpdated_consumer_skips_invalid_payload_without_persisting()
    {
        var repository = CreateRepository();
        var publisher = CreatePublisher();
        var sut = new GameStateUpdatedHandler(repository, CreateMessagingOptions(), publisher, NullLogger<GameStateUpdatedHandler>.Instance);
        var invalidMessage = BuildUpdatedEvent() with { EventId = string.Empty };

        await sut.HandleAsync(new GameStateUpdatedCommand(invalidMessage), CancellationToken.None);

        Assert.Empty(repository.Writes);
        Assert.Empty(publisher.UpdatedNotifications);
    }
}
