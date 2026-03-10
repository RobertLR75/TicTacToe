using GameService.Models;
using GameService.Services;
using SharedLibrary.PostgreSql.EntityFramework;

namespace GameService.Endpoints.Games.List;

public sealed record ListGamesQuery(GameStatus Status, int Page, int PageSize) : IRequest<IEnumerable<Game>>;

public class ListGamesQueryHandler(IPostgresSqlStorageService<Game> gameStore) 
    : IRequestHandler<ListGamesQuery, IEnumerable<Game>>
{
    public async Task<IEnumerable<Game>> HandleAsync(ListGamesQuery request, CancellationToken ct = default)
    {
        var specification = new SearchByStatusSpecification(request.Status, request.Page, request.PageSize);
        return await gameStore.SearchAsync(specification, ct);
    }
}

