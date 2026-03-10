using GameService.Endpoints.Games.UpdateStatus;
using GameService.Models;
using GameService.Services;
using NSubstitute;
using Xunit;

namespace GameService.UnitTests;

public class UpdateGameStatusCommandHandlerUnitTests : GameServiceUnitTestBase
{
    [Fact]
    public async Task HandleAsync_returns_not_found_when_game_does_not_exist()
    {
        var store = CreateStore();
        store.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Game?)null);
        var validator = CreateStatusValidator(GameStatusUpdateResult.SuccessResult(Guid.NewGuid(), GameStatus.Created, DateTimeOffset.UtcNow));
        var publisher = CreatePublisher();
        var sut = new UpdateGameStatusHandler(store, validator, publisher);

        var result = await sut.HandleAsync(new UpdateGameStatusCommand(Guid.NewGuid(), GameStatus.Active));

        Assert.False(result.Succeeded);
        Assert.True(result.NotFound);
    }

    [Fact]
    public async Task HandleAsync_updates_game_when_validator_allows_transition()
    {
        var game = CreateGame(GameStatus.Created);
        var store = CreateStore();
        store.GetAsync(game.Id, Arg.Any<CancellationToken>()).Returns(game);
        var validator = new ValidateGameStatusCommand.ValidateGameStatusCommandHandler();
        var publisher = CreatePublisher();
        var sut = new UpdateGameStatusHandler(store, validator, publisher);

        var result = await sut.HandleAsync(new UpdateGameStatusCommand(game.Id, GameStatus.Active));

        Assert.True(result.Succeeded);
        Assert.Equal(GameStatus.Active, game.Status);
        Assert.NotNull(game.UpdatedAt);
        await store.Received(1).UpdateAsync(game, Arg.Any<CancellationToken>());
        await publisher.Received(1).PublishStatusUpdatedAsync(Arg.Is<GameStatusUpdatedEvent>(e => e.GameId == game.Id && e.NewStatus == GameStatus.Active.ToString()), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_returns_invalid_and_does_not_update_when_validator_rejects()
    {
        var game = CreateGame(GameStatus.Created);
        var store = CreateStore();
        store.GetAsync(game.Id, Arg.Any<CancellationToken>()).Returns(game);
        var validator = CreateStatusValidator(GameStatusUpdateResult.InvalidStatusResult());
        var publisher = CreatePublisher();
        var sut = new UpdateGameStatusHandler(store, validator, publisher);

        var result = await sut.HandleAsync(new UpdateGameStatusCommand(game.Id, GameStatus.Completed));

        Assert.False(result.Succeeded);
        Assert.True(result.InvalidStatus);
        Assert.Equal(GameStatus.Created, game.Status);
        await store.DidNotReceive().UpdateAsync(Arg.Any<Game>(), Arg.Any<CancellationToken>());
        await publisher.DidNotReceive().PublishStatusUpdatedAsync(Arg.Any<GameStatusUpdatedEvent>(), Arg.Any<CancellationToken>());
    }
}
