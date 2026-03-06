using GameStateService.Endpoints.Games.MakeMove;
using GameStateService.Models;
using Xunit;

namespace GameStateService.Tests;

public class MoveOutcomeRequestHandlersUnitTests
{
    [Fact]
    public async Task CheckWinnerRequestHandler_returns_winner_for_matching_row()
    {
        var board = new Board();
        board.SetCell(0, 0, PlayerMark.X);
        board.SetCell(0, 1, PlayerMark.X);
        board.SetCell(0, 2, PlayerMark.X);
        var sut = new CheckWinnerRequestHandler();

        var result = await sut.HandleAsync(new CheckWinnerRequest(board));

        Assert.Equal(PlayerMark.X, result.Winner);
    }

    [Fact]
    public async Task CheckWinnerRequestHandler_returns_none_when_no_winner_exists()
    {
        var board = new Board();
        board.SetCell(0, 0, PlayerMark.X);
        board.SetCell(0, 1, PlayerMark.O);
        board.SetCell(0, 2, PlayerMark.X);
        var sut = new CheckWinnerRequestHandler();

        var result = await sut.HandleAsync(new CheckWinnerRequest(board));

        Assert.Equal(PlayerMark.None, result.Winner);
    }

    [Fact]
    public async Task CheckDrawRequestHandler_returns_true_for_full_board()
    {
        var board = new Board();
        var marks = new[]
        {
            PlayerMark.X, PlayerMark.O, PlayerMark.X,
            PlayerMark.O, PlayerMark.X, PlayerMark.O,
            PlayerMark.O, PlayerMark.X, PlayerMark.O
        };

        var index = 0;
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                board.SetCell(row, col, marks[index++]);
            }
        }

        var sut = new CheckDrawRequestHandler();

        var result = await sut.HandleAsync(new CheckDrawRequest(board));

        Assert.True(result.IsDraw);
    }

    [Fact]
    public async Task CheckDrawRequestHandler_returns_false_when_board_has_empty_cell()
    {
        var board = new Board();
        board.SetCell(0, 0, PlayerMark.X);
        var sut = new CheckDrawRequestHandler();

        var result = await sut.HandleAsync(new CheckDrawRequest(board));

        Assert.False(result.IsDraw);
    }
}
