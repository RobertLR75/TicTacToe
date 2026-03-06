using GameStateService.Endpoints.Games.Create;
using GameStateService.Endpoints.Games.Get;
using GameStateService.Endpoints.Games.MakeMove;
using GameStateService.Models;
using GameStateService.Services;
using Xunit;

namespace GameStateService.Tests;

public class RequestHandlersUnitTests
{
    [Fact]
    public async Task CreateGameCommandHandler_publishes_event_after_create()
    {
        var repository = new FakeRepository();
        var publisher = new FakePublisher();
        var sut = new CreateGameCommandHandler(repository, publisher);

        var response = await sut.HandleAsync(new CreateGameCommand());

        Assert.False(string.IsNullOrWhiteSpace(response.GameId));
        Assert.Equal(1, repository.CreateCalls);
        Assert.Equal(1, publisher.CreatedPublishCalls);
    }

    [Fact]
    public async Task GetGameQueryHandler_returns_not_found_when_game_missing()
    {
        var repository = new FakeRepository();
        var sut = new GetGameQueryHandler(repository);

        var result = await sut.HandleAsync(new GetGameQuery("missing"));

        Assert.False(result.Found);
        Assert.Null(result.Response);
    }

    [Fact]
    public async Task MakeMoveCommandHandler_returns_success_and_publishes_update()
    {
        var repository = new FakeRepository();
        var publisher = new FakePublisher();
        var gameLogicHandler = CreateGameLogicHandler();
        var sut = new MakeMoveCommandHandler(repository, gameLogicHandler, publisher);
        var game = repository.CreateGame();

        var result = await sut.HandleAsync(new MakeMoveCommand(game.GameId, 0, 0));

        Assert.Equal(MakeMoveCommandStatus.Success, result.Status);
        Assert.NotNull(result.Game);
        Assert.Equal(1, repository.UpdateCalls);
        Assert.Equal(1, publisher.UpdatedPublishCalls);
    }

    [Fact]
    public async Task MakeMoveCommandHandler_returns_not_found_and_skips_publish_when_game_missing()
    {
        var repository = new FakeRepository();
        var publisher = new FakePublisher();
        var gameLogicHandler = CreateGameLogicHandler();
        var sut = new MakeMoveCommandHandler(repository, gameLogicHandler, publisher);

        var result = await sut.HandleAsync(new MakeMoveCommand("missing", 0, 0));

        Assert.Equal(MakeMoveCommandStatus.NotFound, result.Status);
        Assert.Equal(0, publisher.UpdatedPublishCalls);
    }

    [Fact]
    public async Task MakeMoveCommandHandler_returns_cell_occupied_and_skips_publish()
    {
        var repository = new FakeRepository();
        var publisher = new FakePublisher();
        var gameLogicHandler = CreateGameLogicHandler();
        var sut = new MakeMoveCommandHandler(repository, gameLogicHandler, publisher);
        var game = repository.CreateGame();
        game.Board.SetCell(0, 0, PlayerMark.X);

        var result = await sut.HandleAsync(new MakeMoveCommand(game.GameId, 0, 0));

        Assert.Equal(MakeMoveCommandStatus.CellOccupied, result.Status);
        Assert.Equal(0, repository.UpdateCalls);
        Assert.Equal(0, publisher.UpdatedPublishCalls);
    }

    [Fact]
    public async Task MakeMoveCommandHandler_returns_game_over_and_skips_publish()
    {
        var repository = new FakeRepository();
        var publisher = new FakePublisher();
        var gameLogicHandler = CreateGameLogicHandler();
        var sut = new MakeMoveCommandHandler(repository, gameLogicHandler, publisher);
        var game = repository.CreateGame();
        game.Winner = PlayerMark.X;

        var result = await sut.HandleAsync(new MakeMoveCommand(game.GameId, 0, 0));

        Assert.Equal(MakeMoveCommandStatus.GameOver, result.Status);
        Assert.Equal(0, repository.UpdateCalls);
        Assert.Equal(0, publisher.UpdatedPublishCalls);
    }

    private sealed class FakeRepository : IGameRepository
    {
        private readonly Dictionary<string, GameState> _games = new();

        public int CreateCalls { get; private set; }
        public int UpdateCalls { get; private set; }

        public GameState CreateGame()
        {
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

        public Task PublishGameCreatedAsync(GameState game, CancellationToken ct = default)
        {
            CreatedPublishCalls++;
            return Task.CompletedTask;
        }

        public Task PublishGameStateUpdatedAsync(GameState game, CancellationToken ct = default)
        {
            UpdatedPublishCalls++;
            return Task.CompletedTask;
        }
    }

    private static GameLogicMoveRequestHandler CreateGameLogicHandler()
    {
        return new GameLogicMoveRequestHandler(new CheckWinnerRequestHandler(), new CheckDrawRequestHandler());
    }
}
