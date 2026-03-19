using System.Diagnostics;
using Service.Contracts.Events;
using SharedLibrary.Services;
using SharedLibrary.Services.Interfaces;

namespace GameService.Features.Games.Endpoints.Create;

public interface ICreateGameEventPublisher : IEventPublisherService<GameCreatedEvent>;

public class CreateGameEventPublisher : PublisherBase<GameCreatedEvent, GameCreated>, ICreateGameEventPublisher
{
    public CreateGameEventPublisher(IEventPublisher eventPublisher, ILogger<PublisherBase<GameCreatedEvent, GameCreated>> logger) : base(eventPublisher, logger)
    {
    }

       protected override async Task<GameCreated?> HandleEventAsync(GameCreatedEvent ev)
    {
        return new GameCreated
        {
            EventId = Guid.NewGuid().ToString("N"),
            SchemaVersion = EventSchemaVersion.V1,
            Id = ev.GameEntity.Id,
            CreatedAt = ev.GameEntity.CreatedAt,
            Player1 = ev.GameEntity.Player1.Id.ToString(),
            OccurredAtUtc = DateTimeOffset.UtcNow,
            CorrelationId = Activity.Current?.TraceId.ToString()
        };
    }
}

