using FastEndpoints;
using GameService.Features.Games.Endpoints.Create;
using GameService.Features.Games.Endpoints.Get;
using GameService.Features.Games.Endpoints.UpdateStatus;
using GameService.Features.Games.Entities;
using NSubstitute;
using SharedLibrary.Interfaces;
using Xunit;

namespace GameService.UnitTests;

public class GameServiceClassContractsUnitTests
{
    [Fact]
    public void Domain_models_expose_expected_properties()
    {
        var playerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var player = new PlayerEntity { Id = playerId, Name = "Alice" };
        var game = new GameEntity
        {
            Id = Guid.NewGuid(),
            Status = GameStatus.Active,
            CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-1),
            UpdatedAt = DateTimeOffset.UtcNow,
            Player1 = player,
            Player2 = new PlayerEntity { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Bob" }
        };

        Assert.Equal(playerId, player.Id);
        Assert.Equal("Alice", player.Name);
        Assert.IsAssignableFrom<IEntity>(game);
        Assert.Equal(GameStatus.Active, game.Status);
        Assert.Equal("Bob", game.Player2!.Name);
    }

    [Fact]
    public async Task Placeholder_event_handlers_complete_successfully()
    {
        var createPublisher = Substitute.For<ICreateGameEventPublisher>();
        var statusPublisher = Substitute.For<IUpdateGameStatusEventPublisher>();
        var createdHandler = new GameCreatedEvent.GameCreatedEventHandler(createPublisher);
        var statusHandler = new GameStatusUpdatedEvent.GameStatusUpdatedEventHandler(statusPublisher);
        var game = new GameEntity
        {
            Id = Guid.NewGuid(),
            Status = GameStatus.Active,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            Player1 = new PlayerEntity { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Alice" }
        };

        await createdHandler.HandleAsync(new GameCreatedEvent
        {
            GameEntity = game
        }, CancellationToken.None);

        await statusHandler.HandleAsync(new GameStatusUpdatedEvent
        {
            GameEntity = game
        }, CancellationToken.None);
    }

    [Fact]
    public void Infrastructure_types_keep_expected_contracts()
    {
        Assert.Contains(typeof(IEvent), typeof(GameCreatedEvent).GetInterfaces());
        Assert.Contains(typeof(IEventHandler<GameCreatedEvent>), typeof(GameCreatedEvent.GameCreatedEventHandler).GetInterfaces());
        Assert.Contains(typeof(IEventHandler<GameStatusUpdatedEvent>), typeof(GameStatusUpdatedEvent.GameStatusUpdatedEventHandler).GetInterfaces());
        Assert.Contains(typeof(ICreateGameEventPublisher), typeof(CreateGameEventPublisher).GetInterfaces());
        Assert.Contains(typeof(IUpdateGameStatusEventPublisher), typeof(UpdateGameStatusEventPublisher).GetInterfaces());
        Assert.Contains(typeof(IGetGameHandler), typeof(GetGameHandler).GetInterfaces());
    }
}
