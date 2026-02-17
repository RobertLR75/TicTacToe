# Request and Response Types

Requests are `class`, responses are `record`.

```csharp
// Request — class (required for FastEndpoints model binding)
public class CreatePersonRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}

// Response — record (immutable output)
public record CreatePersonResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
}
```

- FastEndpoints model binding requires classes with settable properties for request types
- Responses use `record` for immutability and value-based equality
- Response properties use API-facing names (e.g. `Name` not `FirstName`/`LastName`, `CreatedDate` not `CreatedAt`)