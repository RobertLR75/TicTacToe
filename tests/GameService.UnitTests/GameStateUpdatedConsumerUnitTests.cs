using GameService.Consumers;
using GameService.Models;
using GameService.Services;
using MassTransit;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Service.Contracts.Events;
using Service.Contracts.Shared;
using Xunit;

namespace GameService.UnitTests;

public sealed class GameStateUpdatedConsumerUnitTests : GameServiceUnitTestBase
{
    [Fact]
    public async Task Consume_updates_game_to_completed_when_game_state_is_over()
    {
        var game = CreateGame(status: GameStatus.Active);
        var store = CreateStore();
        store.GetAsync(game.Id, Arg.Any<CancellationToken>()).Returns(game);
        var publisher = CreatePublisher();
        var logger = Substitute.For<ILogger<GameStateUpdatedConsumer>>();
        var sut = new GameStateUpdatedConsumer(store, publisher, logger);
        var context = CreateConsumeContext(new GameStateUpdated
        {
            EventId = Guid.NewGuid().ToString("N"),
            SchemaVersion = "1.0",
            GameId = game.Id.ToString("D"),
            CurrentPlayer = PlayerMarkEnum.X,
            Winner = PlayerMarkEnum.X,
            IsDraw = false,
            IsOver = true,
            Board = [],
            OccurredAtUtc = DateTimeOffset.UtcNow
        });

        await sut.Consume(context);

        Assert.Equal(GameStatus.Completed, game.Status);
        Assert.NotNull(game.UpdatedAt);
        await store.Received(1).UpdateAsync(game, Arg.Any<CancellationToken>());
        await publisher.Received(1).PublishEventAsync(
            Arg.Is<GameStatusUpdated>(e => e.GameId == game.Id && e.NewStatus == GameStatusEnum.Completed),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Consume_does_nothing_when_game_state_is_not_over()
    {
        var game = CreateGame(status: GameStatus.Active);
        var store = CreateStore();
        var publisher = CreatePublisher();
        var logger = Substitute.For<ILogger<GameStateUpdatedConsumer>>();
        var sut = new GameStateUpdatedConsumer(store, publisher, logger);
        var context = CreateConsumeContext(new GameStateUpdated
        {
            EventId = Guid.NewGuid().ToString("N"),
            SchemaVersion = "1.0",
            GameId = game.Id.ToString("D"),
            CurrentPlayer = PlayerMarkEnum.O,
            Winner = PlayerMarkEnum.None,
            IsDraw = false,
            IsOver = false,
            Board = [],
            OccurredAtUtc = DateTimeOffset.UtcNow
        });

        await sut.Consume(context);

        await store.DidNotReceive().GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await store.DidNotReceive().UpdateAsync(Arg.Any<Game>(), Arg.Any<CancellationToken>());
        await publisher.DidNotReceive().PublishEventAsync(Arg.Any<GameStatusUpdated>(), Arg.Any<CancellationToken>());
    }

    private static ConsumeContext<GameStateUpdated> CreateConsumeContext(GameStateUpdated message)
    {
        var context = Substitute.For<ConsumeContext<GameStateUpdated>>();
        context.Message.Returns(message);
        context.CancellationToken.Returns(CancellationToken.None);
        return context;
    }
}
