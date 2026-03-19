using GameStateService.Features.GameStates.Entities;
using GameStateService.Services;
using SharedLibrary.Services.Interfaces;

namespace GameStateService.Features.GameStates.Endpoints.Get;

public interface IGetGameHandler : IRequestHandler<GetGameQuery, GameEntity>;

public sealed record GetGameQuery(string GameId) : IRequest<GameEntity>, IRequest<Service.Contracts.Responses.GetGameStateResponse>;

public sealed class GetGameHandler(IGameRepository repository) : IGetGameHandler
{
    public async Task<GameEntity> HandleAsync(GetGameQuery request, CancellationToken ct = default)
        => await repository.GetGameAsync(request.GameId, ct)
           ?? throw new InvalidOperationException("Game state not found.");
}
