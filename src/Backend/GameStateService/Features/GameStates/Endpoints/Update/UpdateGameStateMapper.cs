using GameStateService.Features.GameStates.Entities;
using Service.Contracts.Requests;
using Service.Contracts.Responses;
using Service.Contracts.Shared;
using SharedLibrary.FastEndpoints;

namespace GameStateService.Features.GameStates.Endpoints.Update;

public sealed class UpdateGameStateMapper : BaseCommandMapper<UpdateGameStateRequest, UpdateGameStateResponse, UpdateGameStateCommand, MakeMoveCommandResult>
{
    public override UpdateGameStateCommand ToCommand(UpdateGameStateRequest req)
        => new(req.GameId, req.Row, req.Col);

    public override UpdateGameStateResponse FromEntity(MakeMoveCommandResult result)
    {
        ArgumentNullException.ThrowIfNull(result.Game);

        return FromGame(result.Game);
    }

    public override Task<UpdateGameStateResponse> FromEntityAsync(MakeMoveCommandResult result, CancellationToken ct)
        => Task.FromResult(FromEntity(result));

    private static UpdateGameStateResponse FromGame(GameEntity game)
        => new()
        {
            GameId = game.GameId,
            CurrentPlayer = (PlayerMarkEnum)game.CurrentPlayer,
            Winner = (PlayerMarkEnum)game.Winner,
            IsDraw = game.IsDraw,
            IsOver = game.IsOver,
            Board = game.Board.GetAllCells()
                .Select(cell => new GameStateCellResponse(cell.Row, cell.Col, (PlayerMarkEnum)cell.Mark))
                .ToList()
        };
}
