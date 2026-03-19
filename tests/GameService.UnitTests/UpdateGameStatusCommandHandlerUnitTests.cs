using GameService.Features.Games.Endpoints.UpdateStatus;
using GameService.Features.Games.Entities;
using GameService.Services;
using NSubstitute;
using Service.Contracts.Events;
using Xunit;

namespace GameService.UnitTests;

public class UpdateGameStatusCommandHandlerUnitTests : GameServiceUnitTestBase
{
    [Fact]
    public async Task HandleAsync_returns_not_found_when_game_does_not_exist()
    {
        var store = CreateStore();
        store.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((GameEntity?)null);
        var validator = CreateStatusValidator(GameStatusUpdateResult.SuccessResult(Guid.NewGuid(), GameStatus.Created, DateTimeOffset.UtcNow));
        var sut = new UpdateGameStatusHandler(store, validator);

        var result = await sut.HandleAsync(new UpdateGameStatusCommand(Guid.NewGuid(), GameStatus.Active));

        Assert.False(result.Succeeded);
        Assert.True(result.NotFound);
    }

    [Fact(Skip = "Success path publishes via internal FastEndpoints events; verify mapping in GameStatusUpdatedEventHandler tests and end-to-end behavior in integration tests.")]
    public async Task HandleAsync_updates_game_when_validator_allows_transition()
    {
        var game = CreateGame();
        var store = CreateStore();
        store.GetAsync(game.Id, Arg.Any<CancellationToken>()).Returns(game);
        var validator = new ValidateGameStatusCommand.ValidateGameStatusCommandHandler();
        var sut = new UpdateGameStatusHandler(store, validator);

        var result = await sut.HandleAsync(new UpdateGameStatusCommand(game.Id, GameStatus.Active));

        Assert.True(result.Succeeded);
        Assert.Equal(GameStatus.Active, game.Status);
        Assert.NotNull(game.UpdatedAt);
        await store.Received(1).UpdateAsync(game, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_returns_invalid_and_does_not_update_when_validator_rejects()
    {
        var game = CreateGame();
        var store = CreateStore();
        store.GetAsync(game.Id, Arg.Any<CancellationToken>()).Returns(game);
        var validator = CreateStatusValidator(GameStatusUpdateResult.InvalidStatusResult());
        var sut = new UpdateGameStatusHandler(store, validator);

        var result = await sut.HandleAsync(new UpdateGameStatusCommand(game.Id, GameStatus.Completed));

        Assert.False(result.Succeeded);
        Assert.True(result.InvalidStatus);
        Assert.Equal(GameStatus.Created, game.Status);
        await store.DidNotReceive().UpdateAsync(Arg.Any<GameEntity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GameStatusUpdatedEventHandler_publishes_mapped_shared_event()
    {
        var publisher = Substitute.For<IUpdateGameStatusEventPublisher>();
        var sut = new GameStatusUpdatedEvent.GameStatusUpdatedEventHandler(publisher);
        var updatedAt = new DateTimeOffset(2026, 3, 11, 12, 0, 0, TimeSpan.Zero);
        var game = CreateGame(GameStatus.Active, updatedAt: updatedAt);

        await sut.HandleAsync(new GameStatusUpdatedEvent { GameEntity = game }, CancellationToken.None);

        await publisher.Received(1).PublishAsync(
            Arg.Is<GameStatusUpdatedEvent>(evt => evt.GameEntity == game),
            Arg.Any<CancellationToken>());
    }
}
