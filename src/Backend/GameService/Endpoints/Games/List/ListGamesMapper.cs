using FastEndpoints;
using GameService.Models;
using Service.Contracts.Requests;
using Service.Contracts.Responses;
using Service.Contracts.Shared;

namespace GameService.Endpoints.Games.List;

public sealed class ListGamesMapper : Mapper<ListGamesRequest, ListGamesResponse, ListGamesQuery>
{
    public override ListGamesQuery ToEntity(ListGamesRequest r)
    {
        return new ListGamesQuery((GameStatus)r.Status, r.Page, r.PageSize);
    }

    public ListGamesResponse FromEntity(IEnumerable<Game> games)
    {
        return new ListGamesResponse
        {
            Games = games.Select(g => new GameDto
            {
                Id = g.Id,
                Status = (GameStatusEnum)g.Status,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt ?? g.CreatedAt,
                Player1 = new PlayerDto
                {
                    Id = g.Player1.Id, 
                    Name = g.Player1.Name
                },
                Player2 = g.Player2 is not null
                    ? new PlayerDto 
                    { 
                        Id = g.Player2.Id, 
                        Name = g.Player2.Name 
                    }
                    : null
            }).ToList()
        };
    }
}

