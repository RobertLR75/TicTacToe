using System.Diagnostics;
using Service.Contracts.Events;
using Service.Contracts.Shared;
using SharedLibrary.Services;
using SharedLibrary.Services.Interfaces;

namespace GameService.Features.Games.Endpoints.UpdateStatus;

public interface IUpdateGameStatusEventPublisher : IEventPublisherService<GameStatusUpdatedEvent>;

public class UpdateGameStatusEventPublisher : PublisherBase<GameStatusUpdatedEvent, GameStatusUpdated>, IUpdateGameStatusEventPublisher
{
    public UpdateGameStatusEventPublisher(IEventPublisher eventPublisher, ILogger<PublisherBase<GameStatusUpdatedEvent, GameStatusUpdated>> logger) : base(eventPublisher, logger)
    {
    }

       protected override async Task<GameStatusUpdated?> HandleEventAsync(GameStatusUpdatedEvent ev)
    {
        return new GameStatusUpdated
        {
            EventId = Guid.NewGuid().ToString("N"),
            SchemaVersion = EventSchemaVersion.V1,
            Id = ev.GameEntity.Id,
            NewStatus = (GameStatusEnum)ev.GameEntity.Status,
            UpdatedAt = ev.GameEntity.UpdatedAt ?? DateTimeOffset.UtcNow,
            OccurredAtUtc = DateTimeOffset.UtcNow,
            CorrelationId = Activity.Current?.TraceId.ToString()
        };
    }
}

