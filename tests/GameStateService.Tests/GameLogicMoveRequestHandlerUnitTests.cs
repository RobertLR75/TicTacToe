using GameStateService.GameState;
using GameStateService.Models;
using Xunit;

namespace GameStateService.Tests;

public sealed class GameLogicMoveRequestHandlerUnitTests
{
    [Fact]
    public async Task HandleAsync_returns_success_for_valid_move_and_updates_board()
    {
        var game = new GameStateService.Models.GameState();
        var sut = CreateSut();

        var result = await sut.HandleAsync(new GameStateService.GameState.GameState(game, 0, 0));

        Assert.Equal(GameLogicMoveStatus.Success, result.Status);
        Assert.Equal(PlayerMark.X, game.Board.GetCell(0, 0).Mark);
        Assert.Equal(PlayerMark.O, game.CurrentPlayer);
    }

    [Fact]
    public async Task HandleAsync_returns_game_over_when_game_is_already_over()
    {
        var game = new GameStateService.Models.GameState { Winner = PlayerMark.X };
        var sut = CreateSut();

        var result = await sut.HandleAsync(new GameStateService.GameState.GameState(game, 0, 0));

        Assert.Equal(GameLogicMoveStatus.GameOver, result.Status);
        Assert.Equal(PlayerMark.None, game.Board.GetCell(0, 0).Mark);
    }

    [Fact]
    public async Task HandleAsync_returns_cell_occupied_when_target_cell_is_taken()
    {
        var game = new GameStateService.Models.GameState();
        game.Board.SetCell(1, 1, PlayerMark.X);
        var sut = CreateSut();

        var result = await sut.HandleAsync(new GameStateService.GameState.GameState(game, 1, 1));

        Assert.Equal(GameLogicMoveStatus.CellOccupied, result.Status);
    }

    [Fact]
    public async Task HandleAsync_sets_winner_over_draw_when_last_move_completes_a_row()
    {
        var game = new GameStateService.Models.GameState();
        game.Board.SetCell(0, 0, PlayerMark.X);
        game.Board.SetCell(0, 1, PlayerMark.X);
        game.CurrentPlayer = PlayerMark.X;
        var sut = CreateSut();

        var result = await sut.HandleAsync(new GameStateService.GameState.GameState(game, 0, 2));

        Assert.Equal(GameLogicMoveStatus.Success, result.Status);
        Assert.Equal(PlayerMark.X, game.Winner);
        Assert.False(game.IsDraw);
    }

    private static GameStateHandler CreateSut()
    {
        return new GameStateHandler(new CheckWinnerHandler(), new CheckDrawHandler());
    }
}
