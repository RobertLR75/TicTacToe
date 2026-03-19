using GameService.Features.Games.Endpoints.Create;
using GameService.Features.Games.Endpoints.Get;
using GameService.Features.Games.Endpoints.UpdateStatus;
using GameService.Features.Games.Entities;
using GameService.Services;
using NSubstitute;
using Service.Contracts.Responses;
using SharedLibrary.PostgreSql.EntityFramework;

namespace GameService.UnitTests;

public sealed class GameServiceUnitTestFixture
{
    public PlayerEntity CreatePlayer(string id = "11111111-1111-1111-1111-111111111111", string name = "Alice")
    {
        return new PlayerEntity
        {
            Id = Guid.Parse(id),
            Name = name
        };
    }

    public GameEntity CreateGame(
        GameStatus status = GameStatus.Created,
        Guid? id = null,
        PlayerEntity? player1 = null,
        PlayerEntity? player2 = null,
        DateTimeOffset? createdAt = null,
        DateTimeOffset? updatedAt = null)
    {
        return new GameEntity
        {
            Id = id ?? Guid.NewGuid(),
            Status = status,
            CreatedAt = createdAt ?? DateTimeOffset.UtcNow,
            UpdatedAt = updatedAt,
            Player1 = player1 ?? CreatePlayer(),
            Player2 = player2
        };
    }

    public IGameStorageService CreateStore()
        => Substitute.For<IGameStorageService>();

    public ICreateGameEventPublisher CreatePublisher()
        => Substitute.For<ICreateGameEventPublisher>();

    public IUpdateUpdateGameStatusCommandHandler CreateStatusValidator(GameStatusUpdateResult result)
        => new StubStatusValidator(result);

    public IGetGameHandler CreateGetGameHandler(GameEntity? game)
    {
        var store = CreateStore();
        store.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(game);
        return new GetGameHandler(store);
    }

    private sealed class StubStatusValidator(GameStatusUpdateResult result) : IUpdateUpdateGameStatusCommandHandler
    {
        public Task<GameStatusUpdateResult> HandleAsync(ValidateGameStatusCommand request, CancellationToken ct = default)
        {
            return Task.FromResult(result);
        }
    }
}
