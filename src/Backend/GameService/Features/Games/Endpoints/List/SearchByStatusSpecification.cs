using Ardalis.Specification;
using GameService.Features.Games.Entities;
using SharedLibrary.Interfaces;

namespace GameService.Features.Games.Endpoints.List;

public class SearchByStatusSpecification : PersistenceSpecification<GameEntity>
{
    public SearchByStatusSpecification(GameStatus status, int page, int pageSize)
    {
        Query.Where(x => x.Status == status)
             .OrderBy(x => x.CreatedAt)
             .ThenBy(x => x.Id)
             .Skip((page - 1) * pageSize)
             .Take(pageSize);
    }
}
