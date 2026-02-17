# C# File Structure

Use file-scoped namespaces (C# 10+):

```csharp
// GOOD
namespace GameService.Models;

public class Board { }

// BAD
namespace GameService.Models
{
    public class Board { }
}
```

- One namespace per file
- No braces around the namespace declaration

## One Type Per File

One public type per file. Filename must match the type name.

Allowed exceptions:
- Private nested types used only within the containing type
- A single enum tightly coupled to the file's primary type (e.g. `GameStatus` alongside `Game`)