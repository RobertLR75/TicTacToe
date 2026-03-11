using FastEndpoints;
using GameService.Endpoints.Games.Create;
using GameService.Endpoints.Games.UpdateStatus;
using GameService.Models;
using GameService.Persistence.Migrations;
using GameService.Services;
using NSubstitute;
using SharedLibrary.FluentMigration;
using SharedLibrary.Interfaces;
using Xunit;
using DomainGame = GameService.Models.Game;
using DomainPlayer = GameService.Models.Player;
using PersistenceGame = GameService.Persistence.Entities.Game;
using PersistencePlayer = GameService.Persistence.Entities.Player;

namespace GameService.UnitTests;

public class GameServiceClassContractsUnitTests
{
    [Fact]
    public void Domain_models_expose_expected_properties()
    {
        var player = new DomainPlayer { Id = "p1", Name = "Alice" };
        var game = new DomainGame
        {
            Id = Guid.NewGuid(),
            Status = GameStatus.Active,
            CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-1),
            UpdatedAt = DateTimeOffset.UtcNow,
            Player1 = player,
            Player2 = new DomainPlayer { Id = "p2", Name = "Bob" }
        };

        Assert.Equal("p1", player.Id);
        Assert.Equal("Alice", player.Name);
        Assert.IsAssignableFrom<IEntity>(game);
        Assert.Equal(GameStatus.Active, game.Status);
        Assert.Equal("Bob", game.Player2!.Name);
    }

    [Fact]
    public void Persistence_entities_expose_expected_properties()
    {
        var player1 = new PersistencePlayer { Id = "p1", Name = "Alice", CreatedAt = DateTimeOffset.UtcNow };
        var player2 = new PersistencePlayer { Id = "p2", Name = "Bob", CreatedAt = DateTimeOffset.UtcNow };
        var game = new PersistenceGame
        {
            Id = Guid.NewGuid().ToString("D"),
            Status = GameStatus.Created.ToString(),
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            Player1Id = player1.Id,
            Player2Id = player2.Id,
            Player1 = player1,
            Player2 = player2
        };

        Assert.Equal(player1.Id, game.Player1Id);
        Assert.Equal(player2.Id, game.Player2Id);
        Assert.Equal("Alice", game.Player1.Name);
        Assert.Equal("Bob", game.Player2!.Name);
    }

    [Fact]
    public async Task Placeholder_event_handlers_complete_successfully()
    {
        var publisher = Substitute.For<IGameEventPublisher>();
        var createdHandler = new GameCreatedEvent.GameCreatedEventHandler(publisher);
        var statusHandler = new GameStatusUpdatedEvent.GameStatusUpdatedEventHandler(publisher);
        var game = new DomainGame
        {
            Id = Guid.NewGuid(),
            Status = GameStatus.Active,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            Player1 = new DomainPlayer { Id = "p1", Name = "Alice" }
        };

        await createdHandler.HandleAsync(new GameCreatedEvent
        {
            Game = game
        }, CancellationToken.None);

        await statusHandler.HandleAsync(new GameStatusUpdatedEvent
        {
            Game = game
        }, CancellationToken.None);
    }

    [Fact]
    public void Infrastructure_types_keep_expected_contracts()
    {
        Assert.Contains(typeof(IEvent), typeof(GameCreatedEvent).GetInterfaces());
        Assert.Contains(typeof(IEventHandler<GameCreatedEvent>), typeof(GameCreatedEvent.GameCreatedEventHandler).GetInterfaces());
        Assert.Contains(typeof(IEventHandler<GameStatusUpdatedEvent>), typeof(GameStatusUpdatedEvent.GameStatusUpdatedEventHandler).GetInterfaces());
        Assert.Contains(typeof(IGameEventPublisher), typeof(MassTransitGameEventPublisher).GetInterfaces());
        Assert.True(typeof(GenericTableMigration<DomainGame>).IsAssignableFrom(typeof(CreateGameAndPlayerTables)));
    }
}
