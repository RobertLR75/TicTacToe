# Events

Event handlers are nested inside their event record — the handler is part of the event's contract.

```csharp
public record PersonCreatedEvent : IEvent
{
    public Guid PersonId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public class PersonCreatedEventHandler(ILogger<PersonCreatedEventHandler> logger)
        : IEventHandler<PersonCreatedEvent>
    {
        public Task HandleAsync(PersonCreatedEvent ev, CancellationToken ct)
        {
            logger.LogInformation("Person created: [{PersonId}]", ev.PersonId);
            return Task.CompletedTask;
        }
    }
}
```

**Publishing from an endpoint:**
```csharp
await PublishAsync(new PersonCreatedEvent { PersonId = result.Id, CreatedAt = result.CreatedAt });
```

**Publishing from a command handler:**
```csharp
try { await new PersonUpdatedEvent { ... }.PublishAsync(ct); }
catch (InvalidOperationException) { } // silent fail — FastEndpoints resolver not initialized in tests
```

- File lives in the operation folder (e.g. `Persons/Create/PersonCreatedEvent.cs`)
- This covers FastEndpoints in-process events (`IEvent`/`IEventHandler`) — distinct from cross-service messaging events (`ISharedEvent`)
- Wrap `PublishAsync` in try/catch `InvalidOperationException` when calling from command handlers (test compatibility)