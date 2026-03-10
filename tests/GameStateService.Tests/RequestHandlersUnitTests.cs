using GameStateService.Endpoints.Games.Create;
using GameStateService.Endpoints.Games.Get;
using GameStateService.Endpoints.Games.MakeMove;
using GameStateService.Models;
using GameStateService.Tests.Testing;
using Xunit;

namespace GameStateService.Tests;

public sealed class RequestHandlersUnitTests : UnitTestBase
{
    [Fact]
    public async Task GetGameHandler_returns_not_found_when_game_missing()
    {
        var repository = new FakeRepository();
        var sut = new GetGame.GetGameHandler(repository);

        var result = await sut.HandleAsync(new GetGame("missing"));

        Assert.False(result.Found);
        Assert.Null(result.Response);
    }

    [Fact]
    public async Task GetGameHandler_returns_mapped_response_when_game_exists()
    {
        var repository = new FakeRepository();
        var game = repository.CreateGame();
        var sut = new GetGame.GetGameHandler(repository);

        var result = await sut.HandleAsync(new GetGame(game.GameId));

        Assert.True(result.Found);
        Assert.NotNull(result.Response);
        Assert.Equal(game.GameId, result.Response!.GameId);
        Assert.Equal(9, result.Response.Board.Count);
    }

    [Fact]
    public async Task MakeMoveHandler_returns_success_and_publishes_update()
    {
        var repository = new FakeRepository();
        var publisher = new FakePublisher();
        var sut = new MakeMoveHandler(repository, CreateGameLogicHandler(), publisher);
        var game = repository.CreateGame();

        var result = await sut.HandleAsync(new MakeMove(game.GameId, 0, 0));

        Assert.Equal(MakeMoveCommandStatus.Success, result.Status);
        Assert.NotNull(result.Game);
        Assert.Equal(1, repository.UpdateCalls);
        Assert.Equal(1, publisher.UpdatedPublishCalls);
    }

    [Fact]
    public async Task MakeMoveHandler_returns_not_found_when_game_missing()
    {
        var repository = new FakeRepository();
        var publisher = new FakePublisher();
        var sut = new MakeMoveHandler(repository, CreateGameLogicHandler(), publisher);

        var result = await sut.HandleAsync(new MakeMove("missing", 0, 0));

        Assert.Equal(MakeMoveCommandStatus.NotFound, result.Status);
        Assert.Equal(0, publisher.UpdatedPublishCalls);
    }

    [Fact]
    public async Task MakeMoveHandler_returns_cell_occupied_when_target_cell_is_taken()
    {
        var repository = new FakeRepository();
        var publisher = new FakePublisher();
        var sut = new MakeMoveHandler(repository, CreateGameLogicHandler(), publisher);
        var game = repository.CreateGame();
        game.Board.SetCell(0, 0, PlayerMark.X);

        var result = await sut.HandleAsync(new MakeMove(game.GameId, 0, 0));

        Assert.Equal(MakeMoveCommandStatus.CellOccupied, result.Status);
        Assert.Equal(0, repository.UpdateCalls);
        Assert.Equal(0, publisher.UpdatedPublishCalls);
    }

    [Fact]
    public async Task MakeMoveHandler_returns_game_over_when_game_is_completed()
    {
        var repository = new FakeRepository();
        var publisher = new FakePublisher();
        var sut = new MakeMoveHandler(repository, CreateGameLogicHandler(), publisher);
        var game = repository.CreateGame();
        game.Winner = PlayerMark.X;

        var result = await sut.HandleAsync(new MakeMove(game.GameId, 0, 0));

        Assert.Equal(MakeMoveCommandStatus.GameOver, result.Status);
        Assert.Equal(0, repository.UpdateCalls);
        Assert.Equal(0, publisher.UpdatedPublishCalls);
    }

    [Fact]
    public async Task GameCreatedEventHandler_publishes_initialized_event_via_publisher()
    {
        var publisher = new FakePublisher();
        var sut = new GameCreatedEvent.GameCreatedEventHandler(publisher);
        var game = new GameStateService.Models.GameState();

        await sut.HandleAsync(new GameCreatedEvent { Game = game }, CancellationToken.None);

        Assert.Equal(1, publisher.InitializedPublishCalls);
        Assert.Equal(game.GameId, publisher.LastGameId);
    }
}
