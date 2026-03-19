using FastEndpoints;
using GameStateService.Features.GameStates.Entities;
using GameStateService.Services;
using SharedLibrary.Interfaces;
using SharedLibrary.Services.Interfaces;

namespace GameStateService.Features.GameStates.Endpoints.Update;

public interface IUpdateGameStateHandler : IRequestHandler<UpdateGameStateCommand, MakeMoveCommandResult>;

public sealed record UpdateGameStateCommand(string GameId, int Row, int Col) : IRequest<MakeMoveCommandResult>, IRequest<Service.Contracts.Responses.UpdateGameStateResponse>;

public sealed record MakeMoveCommandResult : IEntityId
{
    public required MakeMoveCommandStatus Status { get; init; }
    public GameEntity? Game { get; init; }
    public Guid Id { get; set; }

    public static MakeMoveCommandResult Success(GameEntity game) => new()
    {
        Status = MakeMoveCommandStatus.Success,
        Id = Guid.TryParse(game.GameId, out var gameId) ? gameId : Guid.Empty,
        Game = game
    };

    public static MakeMoveCommandResult NotFound() => new() { Status = MakeMoveCommandStatus.NotFound, Id = Guid.Empty };

    public static MakeMoveCommandResult GameOver() => new() { Status = MakeMoveCommandStatus.GameOver, Id = Guid.Empty };

    public static MakeMoveCommandResult CellOccupied() => new() { Status = MakeMoveCommandStatus.CellOccupied, Id = Guid.Empty };
}

public sealed class UpdateGameStateHandler(
    IGameRepository repository,
    IRequestHandler<ApplyMove, GameLogicMoveResult> gameLogicMoveHandler)
    : IUpdateGameStateHandler
{
    public async Task<MakeMoveCommandResult> HandleAsync(UpdateGameStateCommand request, CancellationToken ct = default)
    {
        var gameState = await repository.GetGameAsync(request.GameId, ct);
        if (gameState is null)
        {
            return MakeMoveCommandResult.NotFound();
        }

        var logicResult = await gameLogicMoveHandler.HandleAsync(new ApplyMove(gameState, request.Row, request.Col), ct);
        if (logicResult.Status == GameLogicMoveStatus.GameOver)
        {
            return MakeMoveCommandResult.GameOver();
        }

        if (logicResult.Status == GameLogicMoveStatus.CellOccupied)
        {
            return MakeMoveCommandResult.CellOccupied();
        }

        await repository.UpdateGameAsync(gameState, ct);

        await new GameStateUpdatedEvent
        {
            GameState = gameState
        }.PublishAsync(Mode.WaitForNone, ct);

        return MakeMoveCommandResult.Success(gameState);
    }
}

public enum MakeMoveCommandStatus
{
    Success,
    NotFound,
    GameOver,
    CellOccupied
}
