using GameService.Endpoints.Games.Create;
using NSubstitute;
using Service.Contracts.Events;
using Xunit;

namespace GameService.UnitTests;

public class CreateGameHandlerUnitTests : GameServiceUnitTestBase
{
    [Fact]
    public async Task GameCreatedEventHandler_publishes_mapped_shared_event()
    {
        var publisher = CreatePublisher();
        var sut = new GameCreatedEvent.GameCreatedEventHandler(publisher);
        var playerId = Guid.NewGuid();
        var game = CreateGame(player1: CreatePlayer(playerId.ToString("D")));

        await sut.HandleAsync(new GameCreatedEvent { Game = game }, CancellationToken.None);

        await publisher.Received(1).PublishEventAsync(
            Arg.Is<GameCreated>(evt =>
                evt.GameId == game.Id &&
                evt.Player1 == game.Player1.Id &&
                evt.CreatedAt == game.CreatedAt &&
                evt.SchemaVersion == "1.0" &&
                !string.IsNullOrWhiteSpace(evt.EventId) &&
                evt.OccurredAtUtc >= game.CreatedAt),
            Arg.Any<CancellationToken>());
    }
}
