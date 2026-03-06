using GameService.Models;
using SharedLibrary.PostgreSql.EntityFramework;

namespace GameService.Services;

public interface IUpdateGameStatusCommandHandler : IRequestHandler<UpdateGameStatusCommand, GameStatusUpdateResult>;

public sealed record UpdateGameStatusCommand(Guid GameId, GameStatus Status) : IRequest<GameStatusUpdateResult>
{
    public sealed class UpdateUpdateGameStatusCommandHandler(IPostgresSqlStorageService<GameModel> gameStore, IUpdateUpdateGameStatusCommandHandler statusValidator)
        : IUpdateGameStatusCommandHandler
    {
        public async Task<GameStatusUpdateResult> HandleAsync(UpdateGameStatusCommand request, CancellationToken ct = default)
        {
            var gameId = request.GameId;

            var game = await gameStore.GetAsync(gameId, ct);

            if (game is null)
                return GameStatusUpdateResult.NotFoundResult();

            var result = await statusValidator.HandleAsync(new ValidateGameStatusCommand(game, request.Status), ct);

            if (!result.Succeeded)
            {
                return result;
            }

            var status = request.Status;


            game.Status = status;
            game.UpdatedAt = DateTimeOffset.UtcNow;
            
            await gameStore.UpdateAsync(game, ct);

            return GameStatusUpdateResult.SuccessResult(gameId, status, game.UpdatedAt);
        }
    }
}

public sealed record GameStatusUpdateResult(bool Succeeded, bool NotFound, bool InvalidStatus, Guid Id, GameStatus? Status, DateTimeOffset? UpdatedAt)
{
    public static GameStatusUpdateResult SuccessResult(Guid id, GameStatus status, DateTimeOffset? updatedAt) => new(true, false, false, id, status, updatedAt);

    public static GameStatusUpdateResult NotFoundResult() => new(false, true, false, Guid.Empty, null, null);

    public static GameStatusUpdateResult InvalidStatusResult() => new(false, false, true, Guid.Empty, null, null);
}