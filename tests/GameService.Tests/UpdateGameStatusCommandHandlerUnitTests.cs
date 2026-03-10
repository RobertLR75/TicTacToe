using GameService.Endpoints.Games.UpdateStatus;
using GameService.Models;
using GameService.Services;
using NSubstitute;
using SharedLibrary.PostgreSql.EntityFramework;
using Xunit;

namespace GameService.Tests;

public class UpdateGameStatusCommandHandlerUnitTests
{
    [Fact]
    public async Task HandleAsync_returns_not_found_when_game_does_not_exist()
    {
        var store = Substitute.For<IPostgresSqlStorageService<Game>>();
        store.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Game?)null);
        var validator = new FakeStatusValidator(GameStatusUpdateResult.SuccessResult(Guid.NewGuid(), GameStatus.Created, DateTimeOffset.UtcNow));
        var publisher = Substitute.For<IGameEventPublisher>();
        var sut = new UpdateGameStatusHandler(store, validator, publisher);

        var result = await sut.HandleAsync(new UpdateGameStatusCommand(Guid.NewGuid(), GameStatus.Active));

        Assert.False(result.Succeeded);
        Assert.True(result.NotFound);
    }

    [Fact]
    public async Task HandleAsync_updates_game_when_validator_allows_transition()
    {
        var game = BuildGame(GameStatus.Created);
        var store = Substitute.For<IPostgresSqlStorageService<Game>>();
        store.GetAsync(game.Id, Arg.Any<CancellationToken>()).Returns(game);
        var validator = new ValidateGameStatusCommand.ValidateGameStatusCommandHandler();
        var publisher = Substitute.For<IGameEventPublisher>();
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
        var game = BuildGame(GameStatus.Created);
        var store = Substitute.For<IPostgresSqlStorageService<Game>>();
        store.GetAsync(game.Id, Arg.Any<CancellationToken>()).Returns(game);
        var validator = new FakeStatusValidator(GameStatusUpdateResult.InvalidStatusResult());
        var publisher = Substitute.For<IGameEventPublisher>();
        var sut = new UpdateGameStatusHandler(store, validator, publisher);

        var result = await sut.HandleAsync(new UpdateGameStatusCommand(game.Id, GameStatus.Completed));

        Assert.False(result.Succeeded);
        Assert.True(result.InvalidStatus);
        Assert.Equal(GameStatus.Created, game.Status);
        await store.DidNotReceive().UpdateAsync(Arg.Any<Game>(), Arg.Any<CancellationToken>());
        await publisher.DidNotReceive().PublishStatusUpdatedAsync(Arg.Any<GameStatusUpdatedEvent>(), Arg.Any<CancellationToken>());
    }

    private static Game BuildGame(GameStatus status)
    {
        return new Game
        {
            Id = Guid.NewGuid(),
            Status = status,
            Player1 = new Player { Id = "p1", Name = "Alice" }
        };
    }

    private sealed class FakeStatusValidator(GameStatusUpdateResult result) : IUpdateUpdateGameStatusCommandHandler
    {
        public Task<GameStatusUpdateResult> HandleAsync(ValidateGameStatusCommand request, CancellationToken ct = default)
        {
            return Task.FromResult(result);
        }
    }
}
