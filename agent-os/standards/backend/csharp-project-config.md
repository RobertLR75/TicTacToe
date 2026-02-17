# C# Project Configuration

All projects target `net10.0`. Do not downgrade or mix target frameworks across projects.

Required `.csproj` settings:
```xml
<TargetFramework>net10.0</TargetFramework>
<ImplicitUsings>enable</ImplicitUsings>
<Nullable>enable</Nullable>
```

- Do not add explicit `using` statements for namespaces in .NET's implicit set (`System`, `System.Linq`, `System.Collections.Generic`, etc.)
- Only add `using` for namespaces outside the implicit set
