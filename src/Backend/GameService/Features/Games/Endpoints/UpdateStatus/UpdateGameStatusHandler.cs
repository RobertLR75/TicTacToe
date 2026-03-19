using FastEndpoints;
using GameService.Features.Games.Entities;
using GameService.Services;
using SharedLibrary.Services.Interfaces;

namespace GameService.Features.Games.Endpoints.UpdateStatus;

public interface IUpdateGameStatusHandler : IRequestHandler<UpdateGameStatusCommand, GameStatusUpdateResult>;
    
public sealed record UpdateGameStatusCommand(Guid GameId, GameStatus Status) : IRequest<GameStatusUpdateResult>;

public sealed record GameStatusUpdateResult : SharedLibrary.Interfaces.IEntityId
{
    public required bool Succeeded { get; init; }
    public required bool NotFound { get; init; }
    public required bool InvalidStatus { get; init; }
    public Guid Id { get; set; }
    public GameStatus? Status { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }

    public static GameStatusUpdateResult SuccessResult(Guid id, GameStatus status, DateTimeOffset? updatedAt) => new()
    {
        Succeeded = true,
        NotFound = false,
        InvalidStatus = false,
        Id = id,
        Status = status,
        UpdatedAt = updatedAt
    };

    public static GameStatusUpdateResult NotFoundResult() => new()
    {
        Succeeded = false,
        NotFound = true,
        InvalidStatus = false,
        Id = Guid.Empty
    };

    public static GameStatusUpdateResult InvalidStatusResult() => new()
    {
        Succeeded = false,
        NotFound = false,
        InvalidStatus = true,
        Id = Guid.Empty
    };
}

public class UpdateGameStatusHandler(
    IGameStorageService gameStore,
    IUpdateUpdateGameStatusCommandHandler statusValidator)
    : IUpdateGameStatusHandler
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
            GameEntity = game,
        }.PublishAsync(Mode.WaitForNone, ct);

        return GameStatusUpdateResult.SuccessResult(gameId, status, game.UpdatedAt);
    }
}
