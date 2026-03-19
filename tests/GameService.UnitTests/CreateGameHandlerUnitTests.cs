using GameService.Features.Games.Endpoints.Create;
using NSubstitute;
using Service.Contracts.Events;
using Xunit;

namespace GameService.UnitTests;

public class CreateGameHandlerUnitTests : GameServiceUnitTestBase
{
    [Fact]
    public async Task GameCreatedEventHandler_publishes_mapped_shared_event()
    {
        var publisher = Substitute.For<ICreateGameEventPublisher>();
        var sut = new GameCreatedEvent.GameCreatedEventHandler(publisher);
        var playerId = Guid.NewGuid();
        var game = CreateGame(player1: CreatePlayer(playerId.ToString("D")));

        await sut.HandleAsync(new GameCreatedEvent { GameEntity = game }, CancellationToken.None);

        await publisher.Received(1).PublishAsync(
            Arg.Is<GameCreatedEvent>(evt => evt.GameEntity == game),
            Arg.Any<CancellationToken>());
    }
}
