using FastEndpoints;
using GameStateService.Features.GameStates.Entities;
using GameStateService.Services;
using SharedLibrary.Services.Interfaces;

namespace GameStateService.Consumers;

public sealed record InitializeGame(string GameId) : IRequest<GameEntity>
{
    public sealed class InitializeGameHandler(IGameRepository repository)
        : IRequestHandler<InitializeGame, GameEntity>
    {
        public async Task<GameEntity> HandleAsync(InitializeGame request, CancellationToken ct = default)
        {
            var game = await repository.CreateGameAsync(request.GameId, ct);

            await new GameInitializedEvent { GameState = game }.PublishAsync(Mode.WaitForNone, ct);

            return game;
        }
    }
}
