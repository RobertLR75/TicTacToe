using GameNotificationService.Services;
using Service.Contracts.Events;
using Service.Contracts.Shared;
using Xunit;

namespace GameNotificationService.UnitTests;

public class GameNotificationMapperUnitTests
{
    [Fact]
    public void TryMap_game_initialized_event_maps_all_fields()
    {
        var message = new GameStateInitialized
        {
            EventId = "evt-1",
            SchemaVersion = "1.0",
            GameId = "game-1",
            CurrentPlayer = PlayerMarkEnum.X,
            Winner = PlayerMarkEnum.X,
            IsDraw = false,
            IsOver = false,
            Board = [new CellEventDto(0, 0, PlayerMarkEnum.X)],
            OccurredAtUtc = DateTimeOffset.UtcNow,
            CorrelationId = "corr-1"
        };

        var mapped = GameNotificationMapper.TryMap(message, out var notification);

        Assert.True(mapped);
        Assert.NotNull(notification);
        Assert.Equal(message.GameId, notification!.GameId);
        Assert.Single(notification.Board);
        Assert.Equal(PlayerMarkEnum.X, notification.Board[0].Mark);
    }

    [Fact]
    public void TryMap_game_initialized_event_returns_false_for_invalid_payload()
    {
        var message = new GameStateInitialized
        {
            EventId = string.Empty,
            SchemaVersion = "1.0",
            GameId = string.Empty,
            CurrentPlayer = PlayerMarkEnum.X,
            Winner = PlayerMarkEnum.X,
            IsDraw = false,
            IsOver = false,
            Board = null!,
            OccurredAtUtc = DateTimeOffset.UtcNow,
            CorrelationId = "corr-invalid"
        };

        var mapped = GameNotificationMapper.TryMap(message, out var notification);

        Assert.False(mapped);
        Assert.Null(notification);
    }

    [Fact]
    public void TryMap_game_state_updated_event_maps_all_fields()
    {
        var message = new GameStateUpdated
        {
            EventId = "evt-2",
            SchemaVersion = "1.0",
            GameId = "game-2",
            CurrentPlayer = PlayerMarkEnum.X,
            Winner = PlayerMarkEnum.X,
            IsDraw = false,
            IsOver = false,
            Board = [new CellEventDto(0, 0, PlayerMarkEnum.X), new CellEventDto(0, 1, PlayerMarkEnum.X)],
            OccurredAtUtc = DateTimeOffset.UtcNow,
            CorrelationId = "corr-2"
        };

        var mapped = GameNotificationMapper.TryMap(message, out var notification);

        Assert.True(mapped);
        Assert.NotNull(notification);
        Assert.Equal(message.GameId, notification!.GameId);
        Assert.Equal(PlayerMarkEnum.X, notification.CurrentPlayer);
        Assert.Equal(2, notification.Board.Count);
        Assert.Equal(PlayerMarkEnum.X, notification.Board[1].Mark);
    }

    [Fact]
    public void TryMap_game_state_updated_event_returns_false_for_invalid_payload()
    {
        var message = new GameStateUpdated
        {
            EventId = string.Empty,
            SchemaVersion = "1.0",
            GameId = string.Empty,
            CurrentPlayer = PlayerMarkEnum.X,
            Winner = PlayerMarkEnum.X,
            IsDraw = false,
            IsOver = false,
            Board = null!,
            OccurredAtUtc = DateTimeOffset.UtcNow,
            CorrelationId = "corr-2"
        };

        var mapped = GameNotificationMapper.TryMap(message, out var notification);

        Assert.False(mapped);
        Assert.Null(notification);
    }
}
