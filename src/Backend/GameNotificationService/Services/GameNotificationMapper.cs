using Service.Contracts.Events;
using Service.Contracts.Notifications;

namespace GameNotificationService.Services;

public static class GameNotificationMapper
{
    public static bool TryMap(GameStateInitialized message, out GameStateInitializedNotification? notification)
    {
        if (!IsValid(message.Id.ToString(), message.EventId, message.Board))
        {
            notification = null;
            return false;
        }

        notification = new GameStateInitializedNotification
        {
            Id = message.Id.ToString(),
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
        if (!IsValid(message.Id.ToString(), message.EventId, message.Board))
        {
            notification = null;
            return false;
        }

        notification = new GameStateUpdatedNotification
        {
            Id = message.Id.ToString(),
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
