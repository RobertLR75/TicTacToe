using GameNotificationService.Configuration;
using GameNotificationService.Consumers;
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
        var sut = new GameStateInitializedConsumer(repository, CreateMessagingOptions(), publisher, NullLogger<GameStateInitializedConsumer>.Instance);

        await sut.ProcessAsync(BuildInitializedEvent(), CancellationToken.None);

        var write = Assert.Single(repository.Writes);
        Assert.Equal("GameStateInitialized", write.EventType);
        var notification = Assert.Single(publisher.InitializedNotifications);
        Assert.Equal(write.GameId, notification.GameId);
    }

    [Fact]
    public async Task GameStateUpdated_consumer_maps_publishes_and_persists_notification()
    {
        var repository = CreateRepository();
        var publisher = CreatePublisher();
        var sut = new GameStateUpdatedConsumer(repository, CreateMessagingOptions(), publisher, NullLogger<GameStateUpdatedConsumer>.Instance);

        await sut.ProcessAsync(BuildUpdatedEvent(), CancellationToken.None);

        var write = Assert.Single(repository.Writes);
        Assert.Equal("GameStateUpdated", write.EventType);
        var notification = Assert.Single(publisher.UpdatedNotifications);
        Assert.Equal(write.GameId, notification.GameId);
    }

    [Fact]
    public async Task GameStateUpdated_consumer_respects_idempotency_when_repository_rejects_duplicate()
    {
        var repository = CreateRepository(rejectDuplicates: true);
        var publisher = CreatePublisher();
        var sut = new GameStateUpdatedConsumer(repository, CreateMessagingOptions(), publisher, NullLogger<GameStateUpdatedConsumer>.Instance);
        var message = BuildUpdatedEvent();

        await sut.ProcessAsync(message, CancellationToken.None);
        await sut.ProcessAsync(message, CancellationToken.None);

        Assert.Single(repository.Writes);
        Assert.Equal(2, publisher.UpdatedNotifications.Count);
    }

    [Fact]
    public async Task GameStateInitialized_consumer_skips_processing_when_toggle_is_off()
    {
        var repository = CreateRepository();
        var publisher = CreatePublisher();
        var sut = new GameStateInitializedConsumer(repository, CreateMessagingOptions(enableEventConsumers: false), publisher, NullLogger<GameStateInitializedConsumer>.Instance);

        await sut.ProcessAsync(BuildInitializedEvent(), CancellationToken.None);

        Assert.Empty(repository.Writes);
        Assert.Empty(publisher.InitializedNotifications);
    }

    [Fact]
    public async Task GameStateInitialized_consumer_skips_invalid_payload_without_persisting()
    {
        var repository = CreateRepository();
        var publisher = CreatePublisher();
        var sut = new GameStateInitializedConsumer(repository, CreateMessagingOptions(), publisher, NullLogger<GameStateInitializedConsumer>.Instance);
        var invalidMessage = BuildInitializedEvent() with { EventId = string.Empty };

        await sut.ProcessAsync(invalidMessage, CancellationToken.None);

        Assert.Empty(repository.Writes);
        Assert.Empty(publisher.InitializedNotifications);
    }

    [Fact]
    public async Task GameStateUpdated_consumer_skips_invalid_payload_without_persisting()
    {
        var repository = CreateRepository();
        var publisher = CreatePublisher();
        var sut = new GameStateUpdatedConsumer(repository, CreateMessagingOptions(), publisher, NullLogger<GameStateUpdatedConsumer>.Instance);
        var invalidMessage = BuildUpdatedEvent() with { EventId = string.Empty };

        await sut.ProcessAsync(invalidMessage, CancellationToken.None);

        Assert.Empty(repository.Writes);
        Assert.Empty(publisher.UpdatedNotifications);
    }
}
