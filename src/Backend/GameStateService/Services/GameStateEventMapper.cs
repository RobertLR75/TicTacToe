using System.Diagnostics;
using Service.Contracts.Events;
using Service.Contracts.Shared;

namespace GameStateService.Services;

public static class GameStateEventMapper
{
    public static GameStateInitialized ToGameStateInitialized(Models.GameState game)
    {
        return new GameStateInitialized
        {
            EventId = Guid.NewGuid().ToString("N"),
            SchemaVersion = EventSchemaVersion.V1,
            GameId = game.GameId,
            CurrentPlayer = (PlayerMarkEnum)game.CurrentPlayer,
            Winner = (PlayerMarkEnum)game.Winner,
            IsDraw = game.IsDraw,
            IsOver = game.IsOver,
            Board = game.Board.GetAllCells().Select(c => new CellEventDto(c.Row, c.Col, (PlayerMarkEnum)c.Mark)).ToList(),
            OccurredAtUtc = DateTimeOffset.UtcNow,
            CorrelationId = Activity.Current?.TraceId.ToString()
        };
    }

    public static GameStateUpdated ToGameStateUpdated(Models.GameState game)
    {
        return new GameStateUpdated
        {
            EventId = Guid.NewGuid().ToString("N"),
            SchemaVersion = EventSchemaVersion.V1,
            GameId = game.GameId,
            CurrentPlayer = (PlayerMarkEnum)game.CurrentPlayer,
            Winner = (PlayerMarkEnum)game.Winner,
            IsDraw = game.IsDraw,
            IsOver = game.IsOver,
            Board = game.Board.GetAllCells().Select(c => new CellEventDto(c.Row, c.Col, (PlayerMarkEnum)c.Mark)).ToList(),
            OccurredAtUtc = DateTimeOffset.UtcNow,
            CorrelationId = Activity.Current?.TraceId.ToString()
        };
    }
}

public static class EventSchemaVersion
{
    public const string V1 = "1.0";
}
