using GameStateService.Contracts.Events;
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
    public async Task CreateGameCommandHandler_creates_game_in_repository()
    {
        var repository = new FakeRepository();
        var sut = new CreateGameCommandHandler(repository);

        var response = await sut.HandleAsync(new CreateGameCommand());

        Assert.False(string.IsNullOrWhiteSpace(response.GameId));
        Assert.Equal(1, repository.CreateCalls);
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

    [Fact]
    public async Task GameCreatedEventHandler_publishes_event_via_publisher()
    {
        var publisher = new FakePublisher();
        var sut = new GameCreatedEventHandler(publisher);
        var game = new GameState();

        await sut.HandleAsync(new GameCreatedEvent { Game = game }, CancellationToken.None);

        Assert.Equal(1, publisher.InitializedPublishCalls);
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
        public int InitializedPublishCalls { get; private set; }
        public int UpdatedPublishCalls { get; private set; }

        public Task PublishEventAsync<T>(T @event, CancellationToken ct = default) where T : class
        {
            if (@event is GameStateInitializedEvent)
                InitializedPublishCalls++;
            else if (@event is GameStateUpdatedEvent)
                UpdatedPublishCalls++;
            return Task.CompletedTask;
        }
    }

    private static GameLogicMoveRequestHandler CreateGameLogicHandler()
    {
        return new GameLogicMoveRequestHandler(new CheckWinnerRequestHandler(), new CheckDrawRequestHandler());
    }
}
