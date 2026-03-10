using GameService.Endpoints.Games.Create;
using GameService.Models;
using NSubstitute;
using Xunit;

namespace GameService.UnitTests;

public class CreateGameHandlerUnitTests : GameServiceUnitTestBase
{
    [Fact]
    public async Task HandleAsync_creates_game_persists_it_and_publishes_event()
    {
        var store = CreateStore();
        var publisher = CreatePublisher();
        var sut = new CreateGameHandler(store, publisher);
        var playerId = Guid.NewGuid();

        var result = await sut.HandleAsync(new CreateGameCommand(playerId, "Alice"));

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(GameStatus.Created, result.Status);
        Assert.Equal(playerId.ToString("D"), result.Player1.Id);
        Assert.Equal("Alice", result.Player1.Name);
        Assert.True(result.CreatedAt <= DateTimeOffset.UtcNow);

        await store.Received(1).CreateAsync(result, Arg.Any<CancellationToken>());
        await publisher.Received(1).PublishGameCreatedAsync(
            Arg.Is<GameCreatedEvent>(evt =>
                evt.GameId == result.Id &&
                evt.Player1Id == result.Player1.Id &&
                evt.CreatedAt == result.CreatedAt),
            Arg.Any<CancellationToken>());
    }
}
