using GameService.Endpoints.Games.UpdateStatus;
using GameService.Models;
using GameService.Services;
using NSubstitute;
using SharedLibrary.PostgreSql.EntityFramework;

namespace GameService.UnitTests;

public sealed class GameServiceUnitTestFixture
{
    public Player CreatePlayer(string id = "p1", string name = "Alice")
    {
        return new Player
        {
            Id = id,
            Name = name
        };
    }

    public Game CreateGame(
        GameStatus status = GameStatus.Created,
        Guid? id = null,
        Player? player1 = null,
        Player? player2 = null,
        DateTimeOffset? createdAt = null,
        DateTimeOffset? updatedAt = null)
    {
        return new Game
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

    public IGameEventPublisher CreatePublisher()
        => Substitute.For<IGameEventPublisher>();

    public IUpdateUpdateGameStatusCommandHandler CreateStatusValidator(GameStatusUpdateResult result)
        => new StubStatusValidator(result);

    private sealed class StubStatusValidator(GameStatusUpdateResult result) : IUpdateUpdateGameStatusCommandHandler
    {
        public Task<GameStatusUpdateResult> HandleAsync(ValidateGameStatusCommand request, CancellationToken ct = default)
        {
            return Task.FromResult(result);
        }
    }
}

