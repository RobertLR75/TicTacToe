# Commands

Use the `ICommand`/`ICommandHandler` pattern when an operation involves multiple orchestrated steps (validate existence, check conflicts, mutate, publish event). Simple single-step operations (Create, Get) call services directly from the endpoint.

```csharp
// Command is a record; handler is a nested class
public record UpdatePersonCommand(PersonModel Person) : ICommand<PersonModel>
{
    public class UpdatePersonCommandHandler(IPersonStorageService service)
        : ICommandHandler<UpdatePersonCommand, PersonModel>
    {
        public async Task<PersonModel> ExecuteAsync(UpdatePersonCommand command, CancellationToken ct)
        {
            // 1. Validate entity exists
            var existing = await service.GetAsync(command.Person.Id, ct)
                ?? throw new ServiceNotFoundException(...);

            // 2. Check for conflicts
            if (await IsDuplicate(command.Person, existing.Id, ct))
                throw new ServiceConflictException(...);

            // 3. Mutate and persist
            existing.FirstName = command.Person.FirstName?.Trim();
            await service.UpdateAsync(existing, ct);

            // 4. Publish event
            await new PersonUpdatedEvent { ... }.PublishAsync(ct);

            return await service.GetAsync(command.Person.Id, ct)!;
        }
    }
}
```

**Endpoint usage:**
```csharp
var result = await new UpdatePersonCommand(entity).ExecuteAsync(cancellationToken);
```

- Catch `ServiceNotFoundException` → 404, `ServiceConflictException` → 409 at the endpoint level
- Handler nested inside command record, in the same file