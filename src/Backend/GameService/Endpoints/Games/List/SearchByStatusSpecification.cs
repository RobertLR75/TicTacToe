using Ardalis.Specification;
using GameService.Models;
using SharedLibrary.Interfaces;

namespace GameService.Endpoints.Games.List;

public class SearchByStatusSpecification : StorageSpecification<GameModel>
{
    public SearchByStatusSpecification(GameStatus status, int page, int pageSize)
    {
        Query.Where(x => x.Status == status)
             .Skip((page - 1) * pageSize)
             .Take(pageSize);
    }
}