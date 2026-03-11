using FastEndpoints;
using GameService.Models;
using Service.Contracts.Requests;
using Service.Contracts.Responses;
using Service.Contracts.Shared;

namespace GameService.Endpoints.Games.UpdateStatus;

public sealed class UpdateGameStatusMapper : Mapper<UpdateGameStatusRequest, UpdateGameStatusResponse, UpdateGameStatusCommand>
{
    public override UpdateGameStatusCommand ToEntity(UpdateGameStatusRequest req)
    {
        if (req is null) throw new ArgumentNullException(nameof(req));

        return new UpdateGameStatusCommand(req.Id, ParseStatus(req.Status));
    }

    public UpdateGameStatusResponse FromEntity(GameStatusUpdateResult result)
    {
        return new UpdateGameStatusResponse
        {
            Id = result.Id,
            Status = result.Status!.Value.ToString(),
            UpdatedAt = result.UpdatedAt!.Value
        };
    }

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
