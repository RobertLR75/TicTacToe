using GameService.Features.Games.Endpoints.UpdateStatus;
using GameService.Features.Games.Entities;
using GameService.Services;
using Xunit;

namespace GameService.UnitTests;

public class ValidateGameStatusCommandHandlerUnitTests : GameServiceUnitTestBase
{
    private readonly ValidateGameStatusCommand.ValidateGameStatusCommandHandler _sut = new();

    [Fact]
    public async Task HandleAsync_returns_success_when_status_is_unchanged()
    {
        var game = CreateGame(GameStatus.Created, updatedAt: DateTimeOffset.UtcNow);

        var result = await _sut.HandleAsync(new ValidateGameStatusCommand(game, GameStatus.Created));

        Assert.True(result.Succeeded);
        Assert.False(result.InvalidStatus);
    }

    [Fact]
    public async Task HandleAsync_returns_success_when_active_status_is_unchanged()
    {
        var game = CreateGame(GameStatus.Active, updatedAt: DateTimeOffset.UtcNow);

        var result = await _sut.HandleAsync(new ValidateGameStatusCommand(game, GameStatus.Active));

        Assert.True(result.Succeeded);
        Assert.False(result.InvalidStatus);
    }

    [Fact]
    public async Task HandleAsync_returns_invalid_when_completed_is_requested_from_active()
    {
        var game = CreateGame(GameStatus.Active, updatedAt: DateTimeOffset.UtcNow);

        var result = await _sut.HandleAsync(new ValidateGameStatusCommand(game, GameStatus.Completed));

        Assert.False(result.Succeeded);
        Assert.True(result.InvalidStatus);
    }

    [Fact]
    public async Task HandleAsync_returns_invalid_when_current_status_is_completed()
    {
        var game = CreateGame(GameStatus.Completed, updatedAt: DateTimeOffset.UtcNow);

        var result = await _sut.HandleAsync(new ValidateGameStatusCommand(game, GameStatus.Active));

        Assert.False(result.Succeeded);
        Assert.True(result.InvalidStatus);
    }

    [Fact]
    public async Task HandleAsync_returns_success_when_transition_is_created_to_active()
    {
        var game = CreateGame(GameStatus.Created, updatedAt: DateTimeOffset.UtcNow);

        var result = await _sut.HandleAsync(new ValidateGameStatusCommand(game, GameStatus.Active));

        Assert.True(result.Succeeded);
        Assert.False(result.InvalidStatus);
    }

    [Fact]
    public async Task HandleAsync_returns_success_for_other_transitions()
    {
        var game = CreateGame(GameStatus.Created, updatedAt: DateTimeOffset.UtcNow);

        var result = await _sut.HandleAsync(new ValidateGameStatusCommand(game, GameStatus.Completed));

        Assert.True(result.Succeeded);
        Assert.False(result.InvalidStatus);
        Assert.Equal(game.Id, result.Id);
        Assert.Equal(game.Status, result.Status);
    }
}
