using System.Diagnostics;
using GameStateService.Configuration;
using GameStateService.Contracts.Events;
using GameStateService.Models;
using MassTransit;
using Microsoft.Extensions.Options;

namespace GameStateService.Services;

public class MassTransitGameEventPublisher(
    IPublishEndpoint publishEndpoint,
    IOptions<MessagingOptions> messagingOptions) : IGameEventPublisher
{
    public async Task PublishGameCreatedAsync(GameState game, CancellationToken ct = default)
    {
        if (!messagingOptions.Value.EnableEventPublishing)
            return;

        await publishEndpoint.Publish(ToGameCreatedEvent(game), ct);
    }

    public async Task PublishGameStateUpdatedAsync(GameState game, CancellationToken ct = default)
    {
        if (!messagingOptions.Value.EnableEventPublishing)
            return;

        await publishEndpoint.Publish(ToGameStateUpdatedEvent(game), ct);
    }

    private static GameCreatedEvent ToGameCreatedEvent(GameState game)
    {
        return new GameCreatedEvent
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

    private static GameStateUpdatedEvent ToGameStateUpdatedEvent(GameState game)
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
