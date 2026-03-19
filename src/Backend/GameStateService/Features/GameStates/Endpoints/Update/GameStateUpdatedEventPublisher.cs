using System.Diagnostics;
using GameStateService.Features.GameStates.Entities;
using Service.Contracts.Events;
using Service.Contracts.Shared;
using SharedLibrary.Services;
using SharedLibrary.Services.Interfaces;

namespace GameStateService.Features.GameStates.Endpoints.Update;

public interface IGameStateUpdatedEventPublisher : IEventPublisherService<GameStateUpdatedEvent>;
public sealed class GameStateUpdatedEventPublisher : PublisherBase<GameStateUpdatedEvent, GameStateUpdated>, IGameStateUpdatedEventPublisher
{
    public GameStateUpdatedEventPublisher(
        IEventPublisher eventPublisher,
        ILogger<PublisherBase<GameStateUpdatedEvent, GameStateUpdated>> logger) : base(eventPublisher, logger)
    {
    }

    protected override Task<GameStateUpdated?> HandleEventAsync(GameStateUpdatedEvent ev)
        => Task.FromResult<GameStateUpdated?>(CreateUpdatedEvent(ev.GameState));

    private static GameStateUpdated CreateUpdatedEvent(GameEntity game)
    {
        return new GameStateUpdated
        {
            EventId = Guid.NewGuid().ToString("N"),
            SchemaVersion = EventSchemaVersion.V1,
            Id = new Guid(game.GameId),
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