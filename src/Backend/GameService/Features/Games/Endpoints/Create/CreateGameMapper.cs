using GameService.Features.Games.Entities;
using Service.Contracts.Requests;
using Service.Contracts.Responses;
using Service.Contracts.Shared;
using SharedLibrary.FastEndpoints;

namespace GameService.Features.Games.Endpoints.Create;

public sealed class CreateGameMapper : BaseCommandMapper<CreateGameRequest, CreateGameResponse, CreateGameCommand, GameEntity>
{
    public override CreateGameCommand ToCommand(CreateGameRequest req) => new(req.PlayerId, req.PlayerName);

    public override CreateGameResponse FromEntity(GameEntity gameEntity)
        => new()
        {
            Id = gameEntity.Id.ToString(),
            Player1 = new PlayerModel
            {
                PlayerId = gameEntity.Player1.Id.ToString(),
                Name = gameEntity.Player1.Name
            },
            Status = GameStatusEnum.Created
        };

    public override Task<CreateGameResponse> FromEntityAsync(GameEntity gameEntity, CancellationToken ct)
        => Task.FromResult(FromEntity(gameEntity));
}
