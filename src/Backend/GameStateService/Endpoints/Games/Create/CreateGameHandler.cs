using FastEndpoints;
using GameStateService.Services;

namespace GameStateService.Endpoints.Games.Create;

public sealed record CreateGame : IRequest<Models.GameState>
{
    public sealed class CreateGameHandler(IGameRepository repository)
        : IRequestHandler<CreateGame, Models.GameState>
    {
        public async Task<Models.GameState> HandleAsync(CreateGame request, CancellationToken ct = default)
        {
            var game = repository.CreateGame();
        
            await new GameCreatedEvent { Game = game }.PublishAsync(Mode.WaitForNone, ct);

            return game;
        }
    }
}
