using GameNotificationService.Hubs;
using GameNotificationService.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Service.Contracts.Notifications;
using Service.Contracts.Shared;
using Xunit;

namespace GameNotificationService.UnitTests;

public sealed class SignalRGameNotificationPublisherUnitTests
{
    [Fact]
    public async Task PublishGameStateInitializedAsync_sends_notification_to_game_group()
    {
        var hubContext = Substitute.For<IHubContext<GameHub>>();
        var clients = Substitute.For<IHubClients>();
        var clientProxy = Substitute.For<IClientProxy>();
        hubContext.Clients.Returns(clients);
        clients.Group("game-1").Returns(clientProxy);
        var sut = new SignalRGameNotificationPublisher(hubContext, NullLogger<SignalRGameNotificationPublisher>.Instance);
        var notification = new GameStateInitializedNotification
        {
            Id = "game-1",
            CurrentPlayer = PlayerMarkEnum.X,
            Winner = PlayerMarkEnum.X,
            IsDraw = false,
            IsOver = false,
            Board = [new CellNotification(0, 0, PlayerMarkEnum.X)]
        };

        await sut.PublishAsync(notification);

        await clientProxy.Received(1).SendCoreAsync(
            "GameStateInitializedNotification",
            Arg.Is<object?[]>(args => args.Length == 1 && ReferenceEquals(args[0], notification)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PublishGameStateUpdatedAsync_sends_notification_to_game_group()
    {
        var hubContext = Substitute.For<IHubContext<GameHub>>();
        var clients = Substitute.For<IHubClients>();
        var clientProxy = Substitute.For<IClientProxy>();
        hubContext.Clients.Returns(clients);
        clients.Group("game-2").Returns(clientProxy);
        var sut = new SignalRGameNotificationPublisher(hubContext, NullLogger<SignalRGameNotificationPublisher>.Instance);
        var notification = new GameStateUpdatedNotification
        {
            Id = "game-2",
            CurrentPlayer = PlayerMarkEnum.O,
            Winner = PlayerMarkEnum.O,
            IsDraw = false,
            IsOver = false,
            Board = [new CellNotification(0, 0, PlayerMarkEnum.X), new CellNotification(0, 1, PlayerMarkEnum.O)]
        };

        await sut.PublishAsync(notification);

        await clientProxy.Received(1).SendCoreAsync(
            "GameStateUpdatedNotification",
            Arg.Is<object?[]>(args => args.Length == 1 && ReferenceEquals(args[0], notification)),
            Arg.Any<CancellationToken>());
    }
}
