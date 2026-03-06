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
        var store = Substitute.For<IPostgresSqlStorageService<GameModel>>();
        store.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((GameModel?)null);
        var validator = new FakeStatusValidator(GameStatusUpdateResult.SuccessResult(Guid.NewGuid(), GameStatus.Created, DateTimeOffset.UtcNow));
        var sut = new UpdateGameStatusCommand.UpdateUpdateGameStatusCommandHandler(store, validator);

        var result = await sut.HandleAsync(new UpdateGameStatusCommand(Guid.NewGuid(), GameStatus.Active));

        Assert.False(result.Succeeded);
        Assert.True(result.NotFound);
    }

    [Fact]
    public async Task HandleAsync_updates_game_when_validator_allows_transition()
    {
        var game = BuildGame(GameStatus.Created);
        var store = Substitute.For<IPostgresSqlStorageService<GameModel>>();
        store.GetAsync(game.Id, Arg.Any<CancellationToken>()).Returns(game);
        var validator = new ValidateGameStatusCommand.ValidateGameStatusCommandHandler();
        var sut = new UpdateGameStatusCommand.UpdateUpdateGameStatusCommandHandler(store, validator);

        var result = await sut.HandleAsync(new UpdateGameStatusCommand(game.Id, GameStatus.Active));

        Assert.True(result.Succeeded);
        Assert.Equal(GameStatus.Active, game.Status);
        Assert.NotNull(game.UpdatedAt);
        await store.Received(1).UpdateAsync(game, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_returns_invalid_and_does_not_update_when_validator_rejects()
    {
        var game = BuildGame(GameStatus.Created);
        var store = Substitute.For<IPostgresSqlStorageService<GameModel>>();
        store.GetAsync(game.Id, Arg.Any<CancellationToken>()).Returns(game);
        var validator = new FakeStatusValidator(GameStatusUpdateResult.InvalidStatusResult());
        var sut = new UpdateGameStatusCommand.UpdateUpdateGameStatusCommandHandler(store, validator);

        var result = await sut.HandleAsync(new UpdateGameStatusCommand(game.Id, GameStatus.Completed));

        Assert.False(result.Succeeded);
        Assert.True(result.InvalidStatus);
        Assert.Equal(GameStatus.Created, game.Status);
        await store.DidNotReceive().UpdateAsync(Arg.Any<GameModel>(), Arg.Any<CancellationToken>());
    }

    private static GameModel BuildGame(GameStatus status)
    {
        return new GameModel
        {
            Id = Guid.NewGuid(),
            Status = status,
            Player1 = new PlayerModel { Id = "p1", Name = "Alice" }
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
