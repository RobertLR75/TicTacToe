using GameStateService.GameState;
using GameStateService.Services;

namespace GameStateService.Endpoints.Games.MakeMove;

public sealed record MakeMove(string GameId, int Row, int Col) : IRequest<MakeMoveCommandResult>;

public sealed record MakeMoveCommandResult
{
    public required MakeMoveCommandStatus Status { get; init; }
    public Models.GameState? Game { get; init; }

    public static MakeMoveCommandResult Success(Models.GameState game) => new()
    {
        Status = MakeMoveCommandStatus.Success,
        Game = game
    };

    public static MakeMoveCommandResult NotFound() => new() { Status = MakeMoveCommandStatus.NotFound };

    public static MakeMoveCommandResult GameOver() => new() { Status = MakeMoveCommandStatus.GameOver };

    public static MakeMoveCommandResult CellOccupied() => new() { Status = MakeMoveCommandStatus.CellOccupied };
}

public sealed class MakeMoveHandler(
    IGameRepository repository,
    IRequestHandler<GameState.GameState, GameLogicMoveResult> gameLogicMoveHandler,
    IGameEventPublisher eventPublisher)
    : IRequestHandler<MakeMove, MakeMoveCommandResult>
{
    public async Task<MakeMoveCommandResult> HandleAsync(MakeMove request, CancellationToken ct = default)
    {
        var game = repository.GetGame(request.GameId);
        if (game is null)
        {
            return MakeMoveCommandResult.NotFound();
        }

        var logicResult = await gameLogicMoveHandler.HandleAsync(new GameState.GameState(game, request.Row, request.Col), ct);
        if (logicResult.Status == GameLogicMoveStatus.GameOver)
        {
            return MakeMoveCommandResult.GameOver();
        }

        if (logicResult.Status == GameLogicMoveStatus.CellOccupied)
        {
            return MakeMoveCommandResult.CellOccupied();
        }

        repository.UpdateGame(game);

        await eventPublisher.PublishEventAsync(GameEventMapper.ToGameStateUpdatedEvent(game), ct);

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
