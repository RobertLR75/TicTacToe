using GameService.Features.Games.Entities;
using GameService.Services;
using Service.Contracts.Responses;
using SharedLibrary.Services.Interfaces;

namespace GameService.Features.Games.Endpoints.Get;

public interface IGetGameHandler : IRequestHandler<GetGameQuery, GameEntity>;

public sealed record GetGameQuery(Guid GameId) :  IRequest<GameEntity>, IRequest<GetGameResponse>;


public sealed class GetGameHandler(IGameStorageService gameStore) : IGetGameHandler
{
    public async Task<GameEntity?> HandleAsync(GetGameQuery request, CancellationToken ct = default)
    {
        var game = await gameStore.GetAsync(request.GameId, ct);
        return  game;
    }
}
