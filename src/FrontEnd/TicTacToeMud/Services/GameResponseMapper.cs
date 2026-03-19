using Service.Contracts.Models;
using Service.Contracts.Responses;
using TicTacToeMud.Models;

namespace TicTacToeMud.Services;

internal static class GameResponseMapper
{
    public static GameResponse ToGameResponse(this GetGameStateResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        return new GameResponse
        {
            GameId = response.GameId,
            CurrentPlayer = (int)response.CurrentPlayer,
            Winner = (int)response.Winner,
            IsDraw = response.IsDraw,
            IsOver = response.IsOver,
            Board = response.Board.Select(cell => new TicTacToeMud.Models.CellDto(cell.Row, cell.Col, (int)cell.Mark)).ToList()
        };
    }
    
    public static GameResponse ToGameResponse(this GameHubModel response)
    {
        ArgumentNullException.ThrowIfNull(response);

        return new GameResponse
        {
            GameId = response.GameId,
            CurrentPlayer = (int)response.CurrentPlayer,
            Winner = (int)response.Winner,
            IsDraw = response.IsDraw,
            IsOver = response.IsOver,
            Board = response.Board.Select(cell => new Models.CellDto(cell.Row, cell.Col, cell.Mark)).ToList()
        };
    }
}
