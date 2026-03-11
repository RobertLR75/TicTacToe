using FastEndpoints;
using GameStateService.Services;

namespace GameStateService.Consumers;

public sealed record InitializeGame : IRequest<Models.GameState>
{
    public sealed class InitializeGameHandler(IGameRepository repository)
        : IRequestHandler<InitializeGame, Models.GameState>
    {
        public async Task<Models.GameState> HandleAsync(InitializeGame request, CancellationToken ct = default)
        {
            var game = repository.CreateGame();

            await new GameInitializedEvent { GameState = game }.PublishAsync(Mode.WaitForNone, ct);

            return game;
        }
    }
}
