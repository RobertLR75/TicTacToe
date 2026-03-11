using GameStateService.Configuration;
using GameStateService.Models;
using GameStateService.Services;
using MassTransit;
using Microsoft.Extensions.Options;
using NSubstitute;
using Service.Contracts.Events;
using Xunit;

namespace GameStateService.Tests;

public sealed class GameStateServiceUnitTests
{
    [Fact]
    public void GameRepository_create_get_update_and_delete_round_trip()
    {
        IGameRepository sut = new GameRepository();

        var game = sut.CreateGame();
        var loaded = sut.GetGame(game.GameId);

        Assert.NotNull(loaded);
        loaded!.Winner = PlayerMark.X;
        sut.UpdateGame(loaded);

        var updated = sut.GetGame(game.GameId);
        Assert.Equal(PlayerMark.X, updated!.Winner);

        sut.DeleteGame(game.GameId);

        Assert.Null(sut.GetGame(game.GameId));
    }

    [Fact]
    public async Task MassTransitGameStateEventPublisher_skips_publish_when_disabled()
    {
        var publishEndpoint = Substitute.For<IPublishEndpoint>();
        var options = Options.Create(new MessagingOptions { EnableEventPublishing = false });
        var sut = new MassTransitGameStateEventPublisher(publishEndpoint, options);

        await sut.PublishEventAsync(new GameStateInitialized
        {
            EventId = "evt-1",
            SchemaVersion = "1.0",
            GameId = "game-1",
            CurrentPlayer = Service.Contracts.Shared.PlayerMarkEnum.X,
            Winner = Service.Contracts.Shared.PlayerMarkEnum.None,
            IsDraw = false,
            IsOver = false,
            Board = [],
            OccurredAtUtc = DateTimeOffset.UtcNow
        });

        await publishEndpoint.DidNotReceiveWithAnyArgs().Publish(default(object)!, default(CancellationToken));
    }
}
