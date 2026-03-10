using GameStateService.Services;

namespace GameStateService.Endpoints.Games.Get;

public sealed record GetGame(string GameId) : IRequest<GetGameQueryResult>
{
    public sealed class GetGameHandler(IGameRepository repository)
        : IRequestHandler<GetGame, GetGameQueryResult>
    {
        public Task<GetGameQueryResult> HandleAsync(GetGame request, CancellationToken ct = default)
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

};

public sealed record GetGameQueryResult
{
    public required bool Found { get; init; }
    public GetGameResponse? Response { get; init; }

    public static GetGameQueryResult NotFound() => new() { Found = false };

    public static GetGameQueryResult Success(GetGameResponse response) => new()
    {
        Found = true,
        Response = response
    };
}
