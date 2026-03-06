using GameStateService.Services;

namespace GameStateService.Endpoints.Games.Get;

public sealed class GetGameQueryHandler(IGameRepository repository)
    : IRequestHandler<GetGameQuery, GetGameQueryResult>
{
    public Task<GetGameQueryResult> HandleAsync(GetGameQuery request, CancellationToken ct = default)
    {
        var game = repository.GetGame(request.GameId);
        if (game is null)
        {
            return Task.FromResult(GetGameQueryResult.NotFound());
        }

        var response = new GetGameResponse
        {
            GameId = game.GameId,
            CurrentPlayer = game.CurrentPlayer,
            Winner = game.Winner,
            IsDraw = game.IsDraw,
            IsOver = game.IsOver,
            Board = game.Board.GetAllCells()
                .Select(c => new CellDto(c.Row, c.Col, c.Mark))
                .ToList()
        };

        return Task.FromResult(GetGameQueryResult.Success(response));
    }
}
