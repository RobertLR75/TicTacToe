using GameService.Endpoints.Games.UpdateStatus;
using GameService.Models;
using GameService.Services;
using SharedLibrary.PostgreSql.EntityFramework;

namespace GameService.UnitTests;

public abstract class GameServiceUnitTestBase
{
    protected GameServiceUnitTestFixture Fixture { get; } = new();

    protected Player CreatePlayer(string id = "p1", string name = "Alice")
        => Fixture.CreatePlayer(id, name);

    protected Game CreateGame(
        GameStatus status = GameStatus.Created,
        Guid? id = null,
        Player? player1 = null,
        Player? player2 = null,
        DateTimeOffset? createdAt = null,
        DateTimeOffset? updatedAt = null)
        => Fixture.CreateGame(status, id, player1, player2, createdAt, updatedAt);

    protected IGameStorageService CreateStore()
        => Fixture.CreateStore();

    protected IGameEventPublisher CreatePublisher()
        => Fixture.CreatePublisher();

    protected IUpdateUpdateGameStatusCommandHandler CreateStatusValidator(GameStatusUpdateResult result)
        => Fixture.CreateStatusValidator(result);
}

