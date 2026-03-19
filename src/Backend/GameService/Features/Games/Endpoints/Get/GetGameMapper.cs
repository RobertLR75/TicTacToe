using GameService.Features.Games.Entities;
using Service.Contracts.Requests;
using Service.Contracts.Responses;
using Service.Contracts.Shared;
using SharedLibrary.FastEndpoints;

namespace GameService.Features.Games.Endpoints.Get;

public sealed class GetGameMapper : BaseQueryMapper<GetGameRequest, GetGameResponse, GetGameQuery, GameEntity>
{
    public override GetGameQuery ToQuery(GetGameRequest req)
        => new(Guid.TryParse(req.GameId, out var gameId) ? gameId : Guid.Empty);

    public override GetGameResponse FromEntity(GameEntity entity)
    {
        return new GetGameResponse
        {
            GameId = entity.Id.ToString(),
            Status = (GameStatusEnum)entity.Status,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Player1 = new PlayerModel()
            {
                PlayerId = entity.Player1.Id.ToString(),
                Name = entity.Player1.Name
            },
            Player2 = entity.Player2 is null
                ? null
                : new PlayerModel
                {
                    PlayerId = entity.Player2?.Id.ToString(),
                    Name = entity.Player2?.Name
                },
        };
    }

    public override Task<GetGameResponse> FromEntityAsync(GameEntity entity, CancellationToken ct)
        => Task.FromResult(FromEntity(entity));
}