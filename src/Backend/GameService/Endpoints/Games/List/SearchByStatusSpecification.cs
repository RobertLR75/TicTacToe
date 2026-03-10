using Ardalis.Specification;
using GameService.Models;
using SharedLibrary.Interfaces;

namespace GameService.Endpoints.Games.List;

public class SearchByStatusSpecification : StorageSpecification<Game>
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