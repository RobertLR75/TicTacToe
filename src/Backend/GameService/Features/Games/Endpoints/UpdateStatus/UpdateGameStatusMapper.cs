using GameService.Features.Games.Entities;
using Service.Contracts.Requests;
using Service.Contracts.Responses;
using Service.Contracts.Shared;
using SharedLibrary.FastEndpoints;

namespace GameService.Features.Games.Endpoints.UpdateStatus;

public sealed class UpdateGameStatusMapper : BaseCommandMapper<UpdateGameStatusRequest, UpdateGameStatusResponse, UpdateGameStatusCommand, GameStatusUpdateResult>
{
    public override UpdateGameStatusCommand ToCommand(UpdateGameStatusRequest req)
    {
        if (req is null) throw new ArgumentNullException(nameof(req));

        return new UpdateGameStatusCommand(req.Id, ParseStatus(req.Status));
    }

    public override UpdateGameStatusResponse FromEntity(GameStatusUpdateResult result)
        => new()
        {
            Id = result.Id,
            Status = result.Status!.Value.ToString(),
            UpdatedAt = result.UpdatedAt!.Value
        };

    public override Task<UpdateGameStatusResponse> FromEntityAsync(GameStatusUpdateResult result, CancellationToken ct)
        => Task.FromResult(FromEntity(result));

    private static GameStatus ParseStatus(GameStatusEnum statusValue)
    {

        switch (statusValue)
        {
            case GameStatusEnum.Active: return GameStatus.Active;
            case GameStatusEnum.Completed: return GameStatus.Completed; 
            default:
                throw new ArgumentException("Invalid status value", nameof(statusValue));
            
        }
    }
}
