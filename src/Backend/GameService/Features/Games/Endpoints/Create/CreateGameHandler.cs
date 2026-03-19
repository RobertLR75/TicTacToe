using FastEndpoints;
using GameService.Features.Games.Entities;
using GameService.Services;
using SharedLibrary.Services.Interfaces;

namespace GameService.Features.Games.Endpoints.Create;

public interface ICreateGameHandler : IRequestHandler<CreateGameCommand, GameEntity>;

public sealed record CreateGameCommand(Guid PlayerId, string PlayerName) : IRequest<GameEntity>;

public sealed class CreateGameHandler(
    IGameStorageService gameStore) 
    : ICreateGameHandler
{
    public async Task<GameEntity> HandleAsync(CreateGameCommand request, CancellationToken ct = default)
    {
        var player = await gameStore.GetPlayerAsync(request.PlayerId, ct)
            ?? new PlayerEntity
            {
                Id = request.PlayerId,
                Name = request.PlayerName
            };

        var game = new GameEntity
        {
            Id = Guid.NewGuid(),
            Player1 = player
        };
        
        await gameStore.CreateAsync(game, ct);
        
        await new GameCreatedEvent
        {
            GameEntity = game,
        }.PublishAsync(Mode.WaitForNone, ct);
        
        return game;
    }
}
