using FastEndpoints;
using GameService.Models;
using GameService.Services;
using SharedLibrary.PostgreSql.EntityFramework;

namespace GameService.Endpoints.Games.Create;

public sealed record CreateGameCommand(Guid PlayerId, string PlayerName) : IRequest<Game>;

public class CreateGameHandler(
    IPostgresSqlStorageService<Game> gameStore,
    IGameEventPublisher eventPublisher) 
    : IRequestHandler<CreateGameCommand, Game>
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

        await eventPublisher.PublishGameCreatedAsync(new GameCreatedEvent
        {
            GameId = game.Id,
            CreatedAt = game.CreatedAt,
            Player1Id = game.Player1.Id
        }, ct);

        return game;
    }
}
