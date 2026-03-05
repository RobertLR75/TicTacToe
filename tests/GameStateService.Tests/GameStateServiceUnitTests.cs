using GameStateService.Models;
using GameStateService.Services;
using Xunit;

namespace GameStateService.Tests;

public class GameStateServiceUnitTests
{
    [Fact]
    public async Task CreateGameAsync_publishes_created_event_after_persistence()
    {
        var repository = new FakeRepository();
        var publisher = new FakePublisher();
        var sut = new GameStateService.Services.GameStateService(repository, new GameLogicService(), publisher);

        var game = await sut.CreateGameAsync();

        Assert.NotNull(game);
        Assert.Equal(1, repository.CreateCalls);
        Assert.Equal(1, publisher.CreatedPublishCalls);
        Assert.Equal(game.GameId, publisher.LastGameId);
    }

    [Fact]
    public async Task CreateGameAsync_does_not_publish_when_create_fails()
    {
        var repository = new FakeRepository { ThrowOnCreate = true };
        var publisher = new FakePublisher();
        var sut = new GameStateService.Services.GameStateService(repository, new GameLogicService(), publisher);

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CreateGameAsync());

        Assert.Equal(0, publisher.CreatedPublishCalls);
    }

    [Fact]
    public async Task MakeMoveAsync_publishes_update_event_after_successful_update()
    {
        var repository = new FakeRepository();
        var publisher = new FakePublisher();
        var sut = new GameStateService.Services.GameStateService(repository, new GameLogicService(), publisher);
        var game = repository.CreateGame();

        var result = await sut.MakeMoveAsync(game.GameId, 0, 0);

        Assert.Equal(GameMoveStatus.Success, result.Status);
        Assert.Equal(1, repository.UpdateCalls);
        Assert.Equal(1, publisher.UpdatedPublishCalls);
        Assert.Equal(game.GameId, publisher.LastGameId);
    }

    [Fact]
    public async Task MakeMoveAsync_does_not_publish_when_game_not_found()
    {
        var repository = new FakeRepository();
        var publisher = new FakePublisher();
        var sut = new GameStateService.Services.GameStateService(repository, new GameLogicService(), publisher);

        var result = await sut.MakeMoveAsync("missing", 0, 0);

        Assert.Equal(GameMoveStatus.NotFound, result.Status);
        Assert.Equal(0, publisher.UpdatedPublishCalls);
    }

    [Fact]
    public async Task MakeMoveAsync_does_not_publish_when_update_fails()
    {
        var repository = new FakeRepository { ThrowOnUpdate = true };
        var publisher = new FakePublisher();
        var sut = new GameStateService.Services.GameStateService(repository, new GameLogicService(), publisher);
        var game = repository.CreateGame();

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.MakeMoveAsync(game.GameId, 0, 0));

        Assert.Equal(0, publisher.UpdatedPublishCalls);
    }

    private sealed class FakeRepository : IGameRepository
    {
        private readonly Dictionary<string, GameState> _games = new();

        public int CreateCalls { get; private set; }
        public int UpdateCalls { get; private set; }
        public bool ThrowOnCreate { get; init; }
        public bool ThrowOnUpdate { get; init; }

        public GameState CreateGame()
        {
            if (ThrowOnCreate)
                throw new InvalidOperationException("create failed");

            CreateCalls++;
            var game = new GameState();
            _games[game.GameId] = game;
            return game;
        }

        public GameState? GetGame(string gameId)
        {
            _games.TryGetValue(gameId, out var game);
            return game;
        }

        public void UpdateGame(GameState game)
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

    private sealed class FakePublisher : IGameEventPublisher
    {
        public int CreatedPublishCalls { get; private set; }
        public int UpdatedPublishCalls { get; private set; }
        public string? LastGameId { get; private set; }

        public Task PublishGameCreatedAsync(GameState game, CancellationToken ct = default)
        {
            CreatedPublishCalls++;
            LastGameId = game.GameId;
            return Task.CompletedTask;
        }

        public Task PublishGameStateUpdatedAsync(GameState game, CancellationToken ct = default)
        {
            UpdatedPublishCalls++;
            LastGameId = game.GameId;
            return Task.CompletedTask;
        }
    }
}
