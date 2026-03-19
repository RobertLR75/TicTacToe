using GameService.Features.Games.Entities;
using GameService.Services;
using SharedLibrary.Interfaces;
using SharedLibrary.Services.Interfaces;

namespace GameService.Features.Games.Endpoints.List;

public interface IListGamesHandler : IRequestHandler<ListGamesQuery, List<GameEntity>>;

public sealed record ListGamesQuery(GameStatus Status, int Page, int PageSize) : IRequest<List<GameEntity>>;

public class ListGamesHandler(IGameStorageService gameStore) 
    : IListGamesHandler
{
    public async Task<List<GameEntity>> HandleAsync(ListGamesQuery request, CancellationToken ct = default)
    {
        var specification = new SearchByStatusSpecification(request.Status, request.Page, request.PageSize);
        var games = await gameStore.SearchAsync(specification, ct);

        return games;
    }
}
