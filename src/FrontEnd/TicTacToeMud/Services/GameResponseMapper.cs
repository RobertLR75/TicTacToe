using Service.Contracts.Responses;
using TicTacToeMud.Models;

namespace TicTacToeMud.Services;

internal static class GameResponseMapper
{
    public static GameResponse ToGameResponse(this GetGameResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        return new GameResponse
        {
            GameId = response.GameId,
            CurrentPlayer = (int)response.CurrentPlayer,
            Winner = (int)response.Winner,
            IsDraw = response.IsDraw,
            IsOver = response.IsOver,
            Board = response.Board.Select(cell => new TicTacToeMud.Models.CellDto(cell.Row, cell.Col, (int)cell.MarkEnum)).ToList()
        };
    }
}
