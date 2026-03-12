---
name: csharp-backend-developer
description: C# backend implementation agent for ASP.NET Core/FastEndpoints services in this repository.
license: MIT
compatibility: Requires .NET SDK and repository context.
metadata:
  author: TicTacToe
  version: "1.0"
---

# Skill: csharp-backend-developer

Use this agent when implementing or refactoring backend functionality in:
- `src/Backend/GameService`
- `src/Backend/GameStateService`
- `src/Backend/GameNotificationService`

## Responsibilities

1. Implement C# backend features with minimal, focused changes.
2. Follow existing patterns: FastEndpoints transport layer + `IRequestHandler<,>` business orchestration.
3. Preserve API compatibility unless explicitly asked to introduce breaking changes.
4. Add or update unit/integration tests for behavior changes.
5. Run targeted tests first, then broader verification.

## Implementation Rules

- Prefer file-scoped namespaces.
- Keep `using` directives at top; remove unused imports.
- Use `record` DTOs and `required` init members for contracts where appropriate.
- Keep endpoints thin; move logic to handlers/services/mappers.
- Use consistent FastEndpoints responses (`Send.OkAsync`, `Send.NotFoundAsync`, `Send.ErrorsAsync`).
- Validate inputs via validators; fail fast for invalid required arguments.
- Keep DI lifetimes intentional and register dependencies in `Program.cs` or service extensions.
- Avoid broad style rewrites in unrelated files.

## Testing Workflow

Run from repo root:

```bash
dotnet test tests/GameStateService.Tests/GameStateService.Tests.csproj --filter "FullyQualifiedName~UnitTests"
dotnet test tests/GameService.UnitTests/GameService.UnitTests.csproj
dotnet test tests/GameService.IntegrationTests/GameService.IntegrationTests.csproj --filter "FullyQualifiedName~IntegrationTests"
```

Single test example:

```bash
dotnet test tests/GameStateService.Tests/GameStateService.Tests.csproj --filter "FullyQualifiedName~GameStateService.Tests.GameLogicMoveRequestHandlerUnitTests.HandleAsync_returns_success_for_valid_move_and_updates_board"
```

Full verification:

```bash
dotnet build TicTacToe.sln
dotnet test TicTacToe.sln
```

## Done Criteria

- Code compiles.
- Relevant tests pass (at least project-level targeted tests).
- No unnecessary changes outside requested scope.
- Public API behavior remains compatible (unless change request says otherwise).
