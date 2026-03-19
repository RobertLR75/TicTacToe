using System.Diagnostics;
using GameStateService.Consumers;
using GameStateService.Features.GameStates.Entities;
using Service.Contracts.Events;
using Service.Contracts.Shared;
using SharedLibrary.Services;
using SharedLibrary.Services.Interfaces;

namespace GameStateService.Services;

public interface IGameInitializedPublisher : IEventPublisherService<GameInitializedEvent>;

public sealed class GameInitializedPublisher : PublisherBase<GameInitializedEvent, GameStateInitialized>, IGameInitializedPublisher
{
    public GameInitializedPublisher(
        IEventPublisher eventPublisher,
        ILogger<PublisherBase<GameInitializedEvent, GameStateInitialized>> logger) : base(eventPublisher, logger)
    {
    }

    protected override Task<GameStateInitialized?> HandleEventAsync(GameInitializedEvent ev)
        => Task.FromResult<GameStateInitialized?>(CreateInitializedEvent(ev.GameState));

    private static GameStateInitialized CreateInitializedEvent(GameEntity game)
    {
        return new GameStateInitialized
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

