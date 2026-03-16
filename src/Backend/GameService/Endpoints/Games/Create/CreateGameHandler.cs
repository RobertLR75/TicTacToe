using FastEndpoints;
using GameService.Models;
using GameService.Services;

namespace GameService.Endpoints.Games.Create;

public interface ICreateGameHandler : IRequestHandler<CreateGameCommand, Game>;

public sealed record CreateGameCommand(Guid PlayerId, string PlayerName) : IRequest<Game>;

public sealed class CreateGameHandler(
    IGameStorageService gameStore) 
    : ICreateGameHandler
{
    public async Task<Game> HandleAsync(CreateGameCommand request, CancellationToken ct = default)
    {
        var playerId = request.PlayerId.ToString("D");
        var player = await gameStore.GetPlayerAsync(playerId, ct)
            ?? new Player
            {
                Id = playerId,
                Name = request.PlayerName
            };

        var game = new Game
        {
            Id = Guid.NewGuid(),
            Player1 = player
        };
        
        await gameStore.CreateGameAsync(game, ct);
        
        await new GameCreatedEvent
        {
            Game = game,
        }.PublishAsync(Mode.WaitForNone, ct);
        
        return game;
    }
}
