using System.Diagnostics;
using GameStateService.Contracts.Events;

namespace GameStateService.Services;

public static class GameEventMapper
{
    public static GameStateInitializedEvent ToGameStateInitializedEvent(Models.GameState game)
    {
        return new GameStateInitializedEvent
        {
            EventId = Guid.NewGuid().ToString("N"),
            SchemaVersion = EventSchemaVersion.V1,
            GameId = game.GameId,
            CurrentPlayer = game.CurrentPlayer,
            Winner = game.Winner,
            IsDraw = game.IsDraw,
            IsOver = game.IsOver,
            Board = game.Board.GetAllCells().Select(c => new CellEventDto(c.Row, c.Col, c.Mark)).ToList(),
            OccurredAtUtc = DateTimeOffset.UtcNow,
            CorrelationId = Activity.Current?.TraceId.ToString()
        };
    }

    public static GameStateUpdatedEvent ToGameStateUpdatedEvent(Models.GameState game)
    {
        return new GameStateUpdatedEvent
        {
            EventId = Guid.NewGuid().ToString("N"),
            SchemaVersion = EventSchemaVersion.V1,
            GameId = game.GameId,
            CurrentPlayer = game.CurrentPlayer,
            Winner = game.Winner,
            IsDraw = game.IsDraw,
            IsOver = game.IsOver,
            Board = game.Board.GetAllCells().Select(c => new CellEventDto(c.Row, c.Col, c.Mark)).ToList(),
            OccurredAtUtc = DateTimeOffset.UtcNow,
            CorrelationId = Activity.Current?.TraceId.ToString()
        };
    }
}

