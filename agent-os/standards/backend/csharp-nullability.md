# C# Nullable Reference Types

Nullability is enabled project-wide. All code must be null-safe.

- Declare variables as nullable (`string?`) only when null is a valid value
- Never use `null!` without an inline comment explaining why it's safe
- Prefer `??`, `?.`, and null-checks over suppression

```csharp
// BAD
string name = user.Name!;

// GOOD
string name = user.Name ?? throw new InvalidOperationException("Name required");
```
