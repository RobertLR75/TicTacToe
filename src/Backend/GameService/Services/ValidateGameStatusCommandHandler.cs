using GameService.Endpoints.Games.UpdateStatus;
using GameService.Models;

namespace GameService.Services;

public interface IUpdateUpdateGameStatusCommandHandler : IRequestHandler<ValidateGameStatusCommand, GameStatusUpdateResult>;

public record ValidateGameStatusCommand(Game Game, GameStatus Status) : IRequest<GameService.Endpoints.Games.UpdateStatus.GameStatusUpdateResult>
{
    
    public class ValidateGameStatusCommandHandler() : IUpdateUpdateGameStatusCommandHandler
    {
        public async Task<GameService.Endpoints.Games.UpdateStatus.GameStatusUpdateResult> HandleAsync(ValidateGameStatusCommand request, CancellationToken ct = default)
        {
            if (request.Game.Status == request.Status)
                return GameService.Endpoints.Games.UpdateStatus.GameStatusUpdateResult.SuccessResult(request.Game.Id, request.Game.Status, request.Game.UpdatedAt);
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