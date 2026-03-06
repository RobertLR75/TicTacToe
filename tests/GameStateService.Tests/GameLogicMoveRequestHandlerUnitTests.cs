using GameStateService.Endpoints.Games.MakeMove;
using GameStateService.Models;
using Xunit;

namespace GameStateService.Tests;

public class GameLogicMoveRequestHandlerUnitTests
{
    [Fact]
    public async Task HandleAsync_returns_success_for_valid_move_and_updates_board()
    {
        var game = new GameState();
        var sut = CreateSut();

        var result = await sut.HandleAsync(new GameLogicMoveRequest(game, 0, 0));

        Assert.Equal(GameLogicMoveStatus.Success, result.Status);
        Assert.Equal(PlayerMark.X, game.Board.GetCell(0, 0).Mark);
        Assert.Equal(PlayerMark.O, game.CurrentPlayer);
    }

    [Fact]
    public async Task HandleAsync_returns_game_over_for_completed_game()
    {
        var game = new GameState { Winner = PlayerMark.X };
        var sut = CreateSut();

        var result = await sut.HandleAsync(new GameLogicMoveRequest(game, 0, 0));

        Assert.Equal(GameLogicMoveStatus.GameOver, result.Status);
        Assert.Equal(PlayerMark.None, game.Board.GetCell(0, 0).Mark);
    }

    [Fact]
    public async Task HandleAsync_returns_cell_occupied_for_taken_cell()
    {
        var game = new GameState();
        game.Board.SetCell(1, 1, PlayerMark.X);
        var sut = CreateSut();

        var result = await sut.HandleAsync(new GameLogicMoveRequest(game, 1, 1));

        Assert.Equal(GameLogicMoveStatus.CellOccupied, result.Status);
    }

    [Fact]
    public async Task HandleAsync_sets_winner_instead_of_draw_when_last_move_completes_a_line()
    {
        var game = new GameState();
        game.Board.SetCell(0, 0, PlayerMark.X);
        game.Board.SetCell(0, 1, PlayerMark.O);
        game.Board.SetCell(0, 2, PlayerMark.X);
        game.Board.SetCell(1, 0, PlayerMark.O);
        game.Board.SetCell(1, 1, PlayerMark.X);
        game.Board.SetCell(1, 2, PlayerMark.O);
        game.Board.SetCell(2, 0, PlayerMark.O);
        game.Board.SetCell(2, 1, PlayerMark.X);
        game.CurrentPlayer = PlayerMark.X;

        var sut = CreateSut();

        var result = await sut.HandleAsync(new GameLogicMoveRequest(game, 2, 2));

        Assert.Equal(GameLogicMoveStatus.Success, result.Status);
        Assert.Equal(PlayerMark.X, game.Winner);
        Assert.False(game.IsDraw);
    }

    private static GameLogicMoveRequestHandler CreateSut()
    {
        return new GameLogicMoveRequestHandler(new CheckWinnerRequestHandler(), new CheckDrawRequestHandler());
    }
}
