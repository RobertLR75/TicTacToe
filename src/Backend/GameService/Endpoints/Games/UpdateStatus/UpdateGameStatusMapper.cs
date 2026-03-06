using FastEndpoints;
using GameService.Contracts;
using GameService.Models;
using GameService.Services;

namespace GameService.Endpoints.Games.UpdateStatus;

public sealed class UpdateGameStatusMapper : Mapper<UpdateGameStatusRequest, UpdateGameStatusResponse, UpdateGameStatusCommand>
{
    public override UpdateGameStatusCommand ToEntity(UpdateGameStatusRequest req)
    {
        if (req is null) throw new ArgumentNullException(nameof(req));

        return new UpdateGameStatusCommand(req.Id, ParseStatus(req.Status));
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
