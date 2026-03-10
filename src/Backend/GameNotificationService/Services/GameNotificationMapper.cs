using GameNotificationService.Notifications;
using Service.Contracts.GameState;

namespace GameNotificationService.Services;

public static class GameNotificationMapper
{
    public static bool TryMap(GameStateInitialized message, out GameStateInitializedNotification? notification)
    {
        if (!IsValid(message.GameId, message.EventId, message.Board))
        {
            notification = null;
            return false;
        }

        notification = new GameStateInitializedNotification
        {
            GameId = message.GameId,
            CurrentPlayer = message.CurrentPlayer,
            Winner = message.Winner,
            IsDraw = message.IsDraw,
            IsOver = message.IsOver,
            Board = message.Board
                .Select(cell => new CellNotification(cell.Row, cell.Col, cell.Mark))
                .ToList()
        };

        return true;
    }

    public static bool TryMap(GameStateUpdated message, out GameStateUpdatedNotification? notification)
    {
        if (!IsValid(message.GameId, message.EventId, message.Board))
        {
            notification = null;
            return false;
        }

        notification = new GameStateUpdatedNotification
        {
            GameId = message.GameId,
            CurrentPlayer = message.CurrentPlayer,
            Winner = message.Winner,
            IsDraw = message.IsDraw,
            IsOver = message.IsOver,
            Board = message.Board
                .Select(cell => new CellNotification(cell.Row, cell.Col, cell.Mark))
                .ToList()
        };

        return true;
    }

    private static bool IsValid(string gameId, string eventId, IReadOnlyCollection<CellEventDto>? board)
    {
        return !string.IsNullOrWhiteSpace(gameId) &&
               !string.IsNullOrWhiteSpace(eventId) &&
               board is not null;
    }
}
