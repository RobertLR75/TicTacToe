using FastEndpoints;
using GameService.Models;
using GameService.Services;
using SharedLibrary.PostgreSql.EntityFramework;

namespace GameService.Endpoints.Games.Create;

public interface ICreateGameHandler : IRequestHandler<CreateGameCommand, Game>;

public sealed record CreateGameCommand(Guid PlayerId, string PlayerName) : IRequest<Game>;

public class CreateGameHandler(
    IGameStorageService gameStore) 
    : ICreateGameHandler
{
    public async Task<Game> HandleAsync(CreateGameCommand request, CancellationToken ct = default)
    {
        var player = new Player
        {
            Id = request.PlayerId.ToString("D"),
            Name = request.PlayerName
        };

        var game = new Game
        {
            Id = Guid.NewGuid(),
            Player1 = player
        };
        
        await gameStore.CreateAsync(game, ct);
        
        await new GameCreatedEvent
        {
            Game = game,
        }.PublishAsync(Mode.WaitForNone, ct);
        
        return game;
    }
}
