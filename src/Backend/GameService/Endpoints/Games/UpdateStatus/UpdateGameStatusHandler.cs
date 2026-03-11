using FastEndpoints;
using GameService.Models;
using GameService.Services;
using SharedLibrary.PostgreSql.EntityFramework;

namespace GameService.Endpoints.Games.UpdateStatus;

public sealed record UpdateGameStatusCommand(Guid GameId, GameStatus Status) : IRequest<GameStatusUpdateResult>;

public sealed record GameStatusUpdateResult(bool Succeeded, bool NotFound, bool InvalidStatus, Guid Id, GameStatus? Status, DateTimeOffset? UpdatedAt)
{
    public static GameStatusUpdateResult SuccessResult(Guid id, GameStatus status, DateTimeOffset? updatedAt) => new(true, false, false, id, status, updatedAt);

    public static GameStatusUpdateResult NotFoundResult() => new(false, true, false, Guid.Empty, null, null);

    public static GameStatusUpdateResult InvalidStatusResult() => new(false, false, true, Guid.Empty, null, null);
}

public class UpdateGameStatusHandler(
    IPostgresSqlStorageService<Game> gameStore,
    IUpdateUpdateGameStatusCommandHandler statusValidator)
    : IRequestHandler<UpdateGameStatusCommand, GameStatusUpdateResult>
{
    public async Task<GameStatusUpdateResult> HandleAsync(UpdateGameStatusCommand request, CancellationToken ct = default)
    {
        var gameId = request.GameId;

        var game = await gameStore.GetAsync(gameId, ct);

        if (game is null)
            return GameStatusUpdateResult.NotFoundResult();

        var result = await statusValidator.HandleAsync(new ValidateGameStatusCommand(game, request.Status), ct);

        if (result.InvalidStatus)
        {
            return result;
        }

        var status = request.Status;

        game.Status = status;
        game.UpdatedAt = DateTimeOffset.UtcNow;

        await gameStore.UpdateAsync(game, ct);

        await new GameStatusUpdatedEvent
        {
            Game = game,
        }.PublishAsync(Mode.WaitForNone, ct);

        return GameStatusUpdateResult.SuccessResult(gameId, status, game.UpdatedAt);
    }
}