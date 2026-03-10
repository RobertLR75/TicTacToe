using GameStateService.Configuration;
using GameStateService.Models;
using GameStateService.Services;
using MassTransit;
using Microsoft.Extensions.Options;
using NSubstitute;
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
    public async Task MassTransitGameEventPublisher_skips_publish_when_disabled()
    {
        var publishEndpoint = Substitute.For<IPublishEndpoint>();
        var options = Options.Create(new MessagingOptions { EnableEventPublishing = false });
        var sut = new MassTransitGameEventPublisher(publishEndpoint, options);

        await sut.PublishEventAsync(new { EventId = "evt-1" });

        await publishEndpoint.DidNotReceiveWithAnyArgs().Publish(default(object)!, default(CancellationToken));
    }
}
