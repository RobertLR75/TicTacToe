using GameService.Features.Games.Entities;
using SharedLibrary.Services.Interfaces;

namespace GameService.Features.Games.Endpoints.UpdateStatus;

public interface IUpdateUpdateGameStatusCommandHandler : IRequestHandler<ValidateGameStatusCommand, GameStatusUpdateResult>;

public record ValidateGameStatusCommand(GameEntity GameEntity, GameStatus Status) : IRequest<GameStatusUpdateResult>
{
    
    public class ValidateGameStatusCommandHandler() : IUpdateUpdateGameStatusCommandHandler
    {
        public async Task<GameStatusUpdateResult> HandleAsync(ValidateGameStatusCommand request, CancellationToken ct = default)
        {
            if (request.GameEntity.Status == request.Status || request.GameEntity.Status == GameStatus.Active && request.Status == GameStatus.Active)
                return GameStatusUpdateResult.SuccessResult(request.GameEntity.Id, request.GameEntity.Status, request.GameEntity.UpdatedAt);
            if (request.GameEntity.Status == GameStatus.Completed || request.GameEntity.Status == GameStatus.Active && request.Status == GameStatus.Completed)
                return GameStatusUpdateResult.InvalidStatusResult();
            return GameStatusUpdateResult.SuccessResult(request.GameEntity.Id, request.GameEntity.Status, request.GameEntity.UpdatedAt);
        }
    }
}