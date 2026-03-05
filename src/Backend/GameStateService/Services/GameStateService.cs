using GameStateService.Models;

namespace GameStateService.Services;

public class GameStateService(
    IGameRepository repository,
    GameLogicService logicService,
    IGameEventPublisher eventPublisher)
{
    public async Task<GameState> CreateGameAsync(CancellationToken ct = default)
    {
        var game = repository.CreateGame();
        await eventPublisher.PublishGameCreatedAsync(game, ct);
        return game;
    }

    public async Task<GameMoveResult> MakeMoveAsync(string gameId, int row, int col, CancellationToken ct = default)
    {
        var game = repository.GetGame(gameId);
        if (game is null)
            return GameMoveResult.NotFound();

        if (game.IsOver)
            return GameMoveResult.GameOver();

        if (!game.Board.IsEmpty(row, col))
            return GameMoveResult.CellOccupied();

        logicService.MakeMove(game, row, col);
        repository.UpdateGame(game);

        await eventPublisher.PublishGameStateUpdatedAsync(game, ct);

        return GameMoveResult.Success(game);
    }
}

public record GameMoveResult
{
    public required GameMoveStatus Status { get; init; }
    public GameState? Game { get; init; }

    public static GameMoveResult Success(GameState game) => new() { Status = GameMoveStatus.Success, Game = game };
    public static GameMoveResult NotFound() => new() { Status = GameMoveStatus.NotFound };
    public static GameMoveResult GameOver() => new() { Status = GameMoveStatus.GameOver };
    public static GameMoveResult CellOccupied() => new() { Status = GameMoveStatus.CellOccupied };
}

public enum GameMoveStatus
{
    Success,
    NotFound,
    GameOver,
    CellOccupied
}
