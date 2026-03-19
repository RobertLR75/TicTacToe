using GameService.Features.Games.Consumers;
using MassTransit;
using NSubstitute;
using Service.Contracts.Events;
using Service.Contracts.Shared;
using Xunit;
using GameStatus = GameService.Features.Games.Entities.GameStatus;

namespace GameService.UnitTests;

public sealed class GameStateUpdatedConsumerUnitTests : GameServiceUnitTestBase
{
    [Fact]
    public async Task Consume_updates_game_to_completed_when_game_state_is_over()
    {
        var handler = Substitute.For<GameService.Features.Games.Endpoints.UpdateStatus.IUpdateGameStatusHandler>();
        var sut = new GameStateUpdatedConsumer(handler);
        var gameId = Guid.NewGuid();
        var context = CreateConsumeContext(new GameStateUpdated
        {
            EventId = Guid.NewGuid().ToString("N"),
            SchemaVersion = "1.0",
            Id = gameId,
            CurrentPlayer = PlayerMarkEnum.X,
            Winner = PlayerMarkEnum.X,
            IsDraw = false,
            IsOver = true,
            Board = [],
            OccurredAtUtc = DateTimeOffset.UtcNow
        });

        await sut.Consume(context);

        await handler.Received(1).HandleAsync(
            Arg.Is<GameService.Features.Games.Endpoints.UpdateStatus.UpdateGameStatusCommand>(cmd =>
                cmd.GameId == gameId && cmd.Status == GameStatus.Completed),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Consume_does_nothing_when_game_state_is_not_over()
    {
        var handler = Substitute.For<GameService.Features.Games.Endpoints.UpdateStatus.IUpdateGameStatusHandler>();
        var sut = new GameStateUpdatedConsumer(handler);
        var context = CreateConsumeContext(new GameStateUpdated
        {
            EventId = Guid.NewGuid().ToString("N"),
            SchemaVersion = "1.0",
            Id = Guid.NewGuid(),
            CurrentPlayer = PlayerMarkEnum.O,
            Winner = PlayerMarkEnum.None,
            IsDraw = false,
            IsOver = false,
            Board = [],
            OccurredAtUtc = DateTimeOffset.UtcNow
        });

        await sut.Consume(context);

        await handler.DidNotReceive().HandleAsync(Arg.Any<GameService.Features.Games.Endpoints.UpdateStatus.UpdateGameStatusCommand>(), Arg.Any<CancellationToken>());
    }

    private static ConsumeContext<GameStateUpdated> CreateConsumeContext(GameStateUpdated message)
    {
        var context = Substitute.For<ConsumeContext<GameStateUpdated>>();
        context.Message.Returns(message);
        context.CancellationToken.Returns(CancellationToken.None);
        return context;
    }
}
