using FastEndpoints.Security;
using GameService.Features.Games.Entities;
using Service.Contracts.Requests;
using Service.Contracts.Responses;
using Service.Contracts.Shared;
using SharedLibrary.FastEndpoints;

namespace GameService.Features.Games.Endpoints.List;

public sealed class ListGamesMapper : BaseQueryMapper<ListGamesRequest, ListGamesResponse, ListGamesQuery, List<GameEntity>>
{
    public override ListGamesQuery ToQuery(ListGamesRequest r) => new((GameStatus)r.Status, r.Page, r.PageSize);

    public override ListGamesResponse FromEntity(List<GameEntity> entity)
        => new()
        {
            entity.Select(g => new GameModel
            {
                GameId = g.Id.ToString(),
                Status = (GameStatusEnum)g.Status,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt,
                Player1 = new PlayerModel()
                {
                    PlayerId = g.Player1.Id.ToString(),
                    Name = g.Player1.Name
                },
                Player2 = g.Player2 is null  ? null : 
                    new PlayerModel
                    {
                        PlayerId = g.Player2?.Id.ToString(),
                        Name = g.Player2?.Name
                    },
            }).ToList()
        };

    public override Task<ListGamesResponse> FromEntityAsync(List<GameEntity> entity, CancellationToken ct)
        => Task.FromResult(FromEntity(entity));
}
