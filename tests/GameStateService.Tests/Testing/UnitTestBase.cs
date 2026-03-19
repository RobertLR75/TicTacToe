using GameStateService.Consumers;
using GameStateService.Features.GameStates.Entities;
using GameStateService.Features.GameStates.Endpoints.Update;
using GameStateService.Services;
using SharedLibrary.Services.Interfaces;

namespace GameStateService.Tests.Testing;

public abstract class UnitTestBase
{
    protected static IRequestHandler<ApplyMove, GameLogicMoveResult> CreateGameLogicHandler()
    {
        return new GameStateHandler(new CheckWinnerHandler(), new CheckDrawHandler());
    }

    protected sealed class FakeRepository : IGameRepository
    {
        private readonly Dictionary<string, GameEntity> _games = new();

        public int CreateCalls { get; private set; }
        public int UpdateCalls { get; private set; }
        public bool ThrowOnCreate { get; init; }
        public bool ThrowOnUpdate { get; init; }

        public Task<GameEntity> CreateGameAsync(string? gameId = null, CancellationToken ct = default)
        {
            if (ThrowOnCreate)
                throw new InvalidOperationException("create failed");

            CreateCalls++;
            var game = new GameEntity
            {
                GameId = string.IsNullOrWhiteSpace(gameId) ? Guid.NewGuid().ToString() : gameId
            };

            _games[game.GameId] = game;
            return Task.FromResult(game);
        }

        public Task<GameEntity?> GetGameAsync(string gameId, CancellationToken ct = default)
        {
            _games.TryGetValue(gameId, out var game);
            return Task.FromResult(game);
        }

        public Task UpdateGameAsync(GameEntity game, CancellationToken ct = default)
        {
            if (ThrowOnUpdate)
                throw new InvalidOperationException("update failed");

            UpdateCalls++;
            _games[game.GameId] = game;
            return Task.CompletedTask;
        }

        public Task DeleteGameAsync(string gameId, CancellationToken ct = default)
        {
            _games.Remove(gameId);
            return Task.CompletedTask;
        }
    }

    protected sealed class FakeInitializedPublisher : IGameInitializedPublisher
    {
        public int InitializedPublishCalls { get; private set; }
        public string? LastGameId { get; private set; }

        public Task PublishAsync(GameInitializedEvent ev, CancellationToken cancellation = default)
        {
            InitializedPublishCalls++;
            LastGameId = ev.GameState.GameId;
            return Task.CompletedTask;
        }
    }

    protected sealed class FakeUpdatedPublisher : IGameStateUpdatedEventPublisher
    {
        public int UpdatedPublishCalls { get; private set; }
        public string? LastGameId { get; private set; }

        public Task PublishAsync(GameStateUpdatedEvent ev, CancellationToken cancellation = default)
        {
            UpdatedPublishCalls++;
            LastGameId = ev.GameState.GameId;
            return Task.CompletedTask;
        }
    }
}
