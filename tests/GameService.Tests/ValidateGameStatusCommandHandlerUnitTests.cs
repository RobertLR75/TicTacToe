using GameService.Models;
using GameService.Services;
using Xunit;

namespace GameService.Tests;

public class ValidateGameStatusCommandHandlerUnitTests
{
    private readonly ValidateGameStatusCommand.ValidateGameStatusCommandHandler _sut = new();

    [Fact]
    public async Task HandleAsync_returns_success_when_status_is_unchanged()
    {
        var game = BuildGame(GameStatus.Created);

        var result = await _sut.HandleAsync(new ValidateGameStatusCommand(game, GameStatus.Created));

        Assert.True(result.Succeeded);
        Assert.False(result.InvalidStatus);
    }

    [Fact]
    public async Task HandleAsync_returns_invalid_when_completed_is_requested_from_active()
    {
        var game = BuildGame(GameStatus.Active);

        var result = await _sut.HandleAsync(new ValidateGameStatusCommand(game, GameStatus.Completed));

        Assert.False(result.Succeeded);
        Assert.True(result.InvalidStatus);
    }

    [Fact]
    public async Task HandleAsync_returns_invalid_when_current_status_is_completed()
    {
        var game = BuildGame(GameStatus.Completed);

        var result = await _sut.HandleAsync(new ValidateGameStatusCommand(game, GameStatus.Active));

        Assert.False(result.Succeeded);
        Assert.True(result.InvalidStatus);
    }

    [Fact]
    public async Task HandleAsync_returns_success_when_transition_is_created_to_active()
    {
        var game = BuildGame(GameStatus.Created);

        var result = await _sut.HandleAsync(new ValidateGameStatusCommand(game, GameStatus.Active));

        Assert.True(result.Succeeded);
        Assert.False(result.InvalidStatus);
    }

    private static Game BuildGame(GameStatus status)
    {
        return new Game
        {
            Id = Guid.NewGuid(),
            Status = status,
            UpdatedAt = DateTimeOffset.UtcNow,
            Player1 = new Player { Id = "p1", Name = "Alice" }
        };
    }
}
