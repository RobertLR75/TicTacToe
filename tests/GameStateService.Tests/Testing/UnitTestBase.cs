using Service.Contracts.Events;
using Service.Contracts.Shared;
using GameStateService.GameState;
using GameStateService.Models;
using GameStateService.Services;

namespace GameStateService.Tests.Testing;

public abstract class UnitTestBase
{
    protected static IRequestHandler<GameStateService.GameState.GameState, GameLogicMoveResult> CreateGameLogicHandler()
    {
        return new GameStateHandler(new CheckWinnerHandler(), new CheckDrawHandler());
    }

    protected sealed class FakeRepository : IGameRepository
    {
        private readonly Dictionary<string, Models.GameState> _games = new();

        public int CreateCalls { get; private set; }
        public int UpdateCalls { get; private set; }
        public bool ThrowOnCreate { get; init; }
        public bool ThrowOnUpdate { get; init; }

        public Models.GameState CreateGame(string? gameId = null)
        {
            if (ThrowOnCreate)
                throw new InvalidOperationException("create failed");

            CreateCalls++;
            var game = new Models.GameState
            {
                GameId = string.IsNullOrWhiteSpace(gameId) ? Guid.NewGuid().ToString() : gameId
            };

            _games[game.GameId] = game;
            return game;
        }

        public Models.GameState? GetGame(string gameId)
        {
            _games.TryGetValue(gameId, out var game);
            return game;
        }

        public void UpdateGame(Models.GameState game)
        {
            if (ThrowOnUpdate)
                throw new InvalidOperationException("update failed");

            UpdateCalls++;
            _games[game.GameId] = game;
        }

        public void DeleteGame(string gameId)
        {
            _games.Remove(gameId);
        }
    }

    protected sealed class FakePublisher : IGameEventPublisher
    {
        public int InitializedPublishCalls { get; private set; }
        public int UpdatedPublishCalls { get; private set; }
        public string? LastGameId { get; private set; }

        public Task PublishEventAsync<T>(T @event, CancellationToken ct = default) where T : class, ISharedEvent
        {
            if (@event is GameStateInitialized initialized)
            {
                InitializedPublishCalls++;
                LastGameId = initialized.GameId;
            }
            else if (@event is GameStateUpdated updated)
            {
                UpdatedPublishCalls++;
                LastGameId = updated.GameId;
            }

            return Task.CompletedTask;
        }
    }
}
