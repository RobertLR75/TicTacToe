using GameStateService.Services;

namespace GameStateService.Endpoints.Games.Create;

public sealed class CreateGameCommandHandler(
    IGameRepository repository,
    IGameEventPublisher eventPublisher)
    : IRequestHandler<CreateGameCommand, CreateGameResponse>
{
    public async Task<CreateGameResponse> HandleAsync(CreateGameCommand request, CancellationToken ct = default)
    {
        var game = repository.CreateGame();
        await eventPublisher.PublishGameCreatedAsync(game, ct);

        return new CreateGameResponse
        {
            GameId = game.GameId
        };
    }
}
