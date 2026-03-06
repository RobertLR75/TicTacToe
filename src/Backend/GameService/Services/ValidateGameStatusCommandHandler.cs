using GameService.Models;

namespace GameService.Services;

public interface IUpdateUpdateGameStatusCommandHandler : IRequestHandler<ValidateGameStatusCommand, GameStatusUpdateResult>;

public record ValidateGameStatusCommand(GameModel Game, GameStatus Status) : IRequest<GameStatusUpdateResult>
{
    
    public class ValidateGameStatusCommandHandler() : IUpdateUpdateGameStatusCommandHandler
    {
        public async Task<GameStatusUpdateResult> HandleAsync(ValidateGameStatusCommand request, CancellationToken ct = default)
        {
            if (request.Game.Status == request.Status)
                return GameStatusUpdateResult.SuccessResult(request.Game.Id, request.Game.Status, request.Game.UpdatedAt);
            else if (request.Game.Status == GameStatus.Active && request.Status == GameStatus.Active)
                return GameStatusUpdateResult.SuccessResult(request.Game.Id, request.Game.Status, request.Game.UpdatedAt);
            else if (request.Game.Status == GameStatus.Completed || request.Game.Status == GameStatus.Active && request.Status == GameStatus.Completed)
                return GameStatusUpdateResult.InvalidStatusResult();
            else
            {
                return GameStatusUpdateResult.SuccessResult(request.Game.Id, request.Game.Status, request.Game.UpdatedAt);
            }
        }
    }
}