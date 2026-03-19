using GameStateService.Features.GameStates.Entities;
using Service.Contracts.Requests;
using Service.Contracts.Responses;
using Service.Contracts.Shared;
using SharedLibrary.FastEndpoints;

namespace GameStateService.Features.GameStates.Endpoints.Get;

public sealed class GetGameStateMapper : BaseQueryMapper<GetGameRequest, GetGameStateResponse, GetGameQuery, GameEntity>
{
    public override GetGameQuery ToQuery(GetGameRequest req)
        => new(req.GameId);

    public override GetGameStateResponse FromEntity(GameEntity entity)
        => new()
        {
            GameId = entity.GameId,
            CurrentPlayer = (PlayerMarkEnum)entity.CurrentPlayer,
            Winner = (PlayerMarkEnum)entity.Winner,
            IsDraw = entity.IsDraw,
            IsOver = entity.IsOver,
            Board = entity.Board.GetAllCells()
                .Select(cell => new GameStateCellResponse(cell.Row, cell.Col, (PlayerMarkEnum)cell.Mark))
                .ToList()
        };

    public override Task<GetGameStateResponse> FromEntityAsync(GameEntity entity, CancellationToken ct)
        => Task.FromResult(FromEntity(entity));
}
