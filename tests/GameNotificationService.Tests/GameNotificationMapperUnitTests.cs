using GameNotificationService.Services;
using Service.Contracts.GameState;
using Xunit;

namespace GameNotificationService.Tests;

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
            CurrentPlayer = 1,
            Winner = 0,
            IsDraw = false,
            IsOver = false,
            Board = [new CellEventDto(0, 0, 1)],
            OccurredAtUtc = DateTimeOffset.UtcNow,
            CorrelationId = "corr-1"
        };

        var mapped = GameNotificationMapper.TryMap(message, out var notification);

        Assert.True(mapped);
        Assert.NotNull(notification);
        Assert.Equal(message.GameId, notification!.GameId);
        Assert.Single(notification.Board);
        Assert.Equal(1, notification.Board[0].Mark);
    }

    [Fact]
    public void TryMap_game_state_updated_event_returns_false_for_invalid_payload()
    {
        var message = new GameStateUpdated
        {
            EventId = string.Empty,
            SchemaVersion = "1.0",
            GameId = string.Empty,
            CurrentPlayer = 1,
            Winner = 0,
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
