using GameStateService.Models;
using GameStateService.Services;

namespace GameStateService.Endpoints.Games.MakeMove;

public sealed record MakeMoveCommand(string GameId, int Row, int Col) : IRequest<MakeMoveCommandResult>;

public sealed record MakeMoveCommandResult
{
    public required MakeMoveCommandStatus Status { get; init; }
    public GameState? Game { get; init; }

    public static MakeMoveCommandResult Success(GameState game) => new()
    {
        Status = MakeMoveCommandStatus.Success,
        Game = game
    };

    public static MakeMoveCommandResult NotFound() => new() { Status = MakeMoveCommandStatus.NotFound };

    public static MakeMoveCommandResult GameOver() => new() { Status = MakeMoveCommandStatus.GameOver };

    public static MakeMoveCommandResult CellOccupied() => new() { Status = MakeMoveCommandStatus.CellOccupied };
}

public sealed class MakeMoveCommandHandler(
    IGameRepository repository,
    IRequestHandler<GameLogicMoveRequest, GameLogicMoveResult> gameLogicMoveHandler,
    IGameEventPublisher eventPublisher)
    : IRequestHandler<MakeMoveCommand, MakeMoveCommandResult>
{
    public async Task<MakeMoveCommandResult> HandleAsync(MakeMoveCommand request, CancellationToken ct = default)
    {
        var game = repository.GetGame(request.GameId);
        if (game is null)
        {
            return MakeMoveCommandResult.NotFound();
        }

        var logicResult = await gameLogicMoveHandler.HandleAsync(new GameLogicMoveRequest(game, request.Row, request.Col), ct);
        if (logicResult.Status == GameLogicMoveStatus.GameOver)
        {
            return MakeMoveCommandResult.GameOver();
        }

        if (logicResult.Status == GameLogicMoveStatus.CellOccupied)
        {
            return MakeMoveCommandResult.CellOccupied();
        }

        repository.UpdateGame(game);

        await eventPublisher.PublishGameStateUpdatedAsync(game, ct);

        return MakeMoveCommandResult.Success(game);
    }
}

public enum MakeMoveCommandStatus
{
    Success,
    NotFound,
    GameOver,
    CellOccupied
}
