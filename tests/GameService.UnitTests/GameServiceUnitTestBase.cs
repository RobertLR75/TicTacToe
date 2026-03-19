using GameService.Features.Games.Endpoints.Create;
using GameService.Features.Games.Endpoints.Get;
using GameService.Features.Games.Endpoints.UpdateStatus;
using GameService.Features.Games.Entities;
using GameService.Services;
using SharedLibrary.PostgreSql.EntityFramework;

namespace GameService.UnitTests;

public abstract class GameServiceUnitTestBase
{
    protected GameServiceUnitTestFixture Fixture { get; } = new();

    protected PlayerEntity CreatePlayer(string id = "p1", string name = "Alice")
        => Fixture.CreatePlayer(id, name);

    protected GameEntity CreateGame(
        GameStatus status = GameStatus.Created,
        Guid? id = null,
        PlayerEntity? player1 = null,
        PlayerEntity? player2 = null,
        DateTimeOffset? createdAt = null,
        DateTimeOffset? updatedAt = null)
        => Fixture.CreateGame(status, id, player1, player2, createdAt, updatedAt);

    protected IGameStorageService CreateStore()
        => Fixture.CreateStore();

    protected ICreateGameEventPublisher CreatePublisher()
        => Fixture.CreatePublisher();

    protected IUpdateUpdateGameStatusCommandHandler CreateStatusValidator(GameStatusUpdateResult result)
        => Fixture.CreateStatusValidator(result);

    protected IGetGameHandler CreateGetGameHandler(GameEntity? game)
        => Fixture.CreateGetGameHandler(game);
}
