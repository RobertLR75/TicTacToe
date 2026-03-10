using FastEndpoints;
using GameService.Models;
using Service.Contracts.CreateGame;
using Service.Contracts.Shared;
using PlayerModel = Service.Contracts.Shared.PlayerModel;

namespace GameService.Endpoints.Games.Create;

public sealed class CreateGameMapper : Mapper<CreateGameRequest, CreateGameResponse, CreateGameCommand>
{
    public override CreateGameCommand ToEntity(CreateGameRequest req)
    {
        return new CreateGameCommand(req.PlayerId, req.PlayerName);
    }

    public CreateGameResponse FromEntity(Game game)
    {
        return new CreateGameResponse
        {
            Id = game.Id,
            Player1 = new PlayerModel
            {
                Id = game.Player1.Id,
                Name = game.Player1.Name
            },
            Status = GameStatusEnum.Created
        };
    }
}


