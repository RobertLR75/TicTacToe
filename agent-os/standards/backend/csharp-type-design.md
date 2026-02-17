# C# Type Design: Record vs Class

Use the distinction consistently — mixing without a rule breaks equality semantics.

| Use `record` | Use `class` |
|---|---|
| Value objects (compared by value, immutable) | Entities (identity, mutable lifecycle) |
| `Move`, `Position`, `Cell` | `Game`, `GameSession` |

```csharp
// Value object → record
public record Move(int Row, int Col);
public record Position(int X, int Y);

// Entity → class
public class Game
{
    public Guid Id { get; init; }
    public Board Board { get; private set; } = new();
}
```

- Records: structurally equal, immutable by default, suitable for DDD value objects
- Classes: reference equal, mutable, suitable for aggregates/entities with lifecycle