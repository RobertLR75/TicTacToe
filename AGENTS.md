# AGENTS.md

Agent playbook for the TicTacToe repository.
This file is intended for coding agents that modify, build, and test code here.

## Repo Snapshot

- Tech stack: .NET 10, ASP.NET Core, FastEndpoints, MassTransit, PostgreSQL, SignalR, MudBlazor.
- Solution: `TicTacToe.sln`.
- Main services:
  - `src/Backend/GameService`
  - `src/Backend/GameStateService`
  - `src/Backend/GameNotificationService`
  - `src/FrontEnd/TicTacToeMud`
- Test projects:
  - `tests/GameService.UnitTests`
  - `tests/GameService.IntegrationTests`
  - `tests/GameStateService.Tests`
  - `tests/GameNotificationService.UnitTests`
  - `tests/GameNotificationService.IntegrationTests`
  - `tests/TicTacToeMud.Tests`

## Rule Files (Cursor/Copilot)

- No Cursor rules were found (`.cursor/rules/` or `.cursorrules`).
- No Copilot instructions file was found (`.github/copilot-instructions.md`).
- If these files are added later, treat them as highest-priority agent instructions.

## Build / Run / Test Commands

Run from repo root unless noted.

### Restore and Build

- Restore all projects:
  - `dotnet restore TicTacToe.sln`
- Build all projects:
  - `dotnet build TicTacToe.sln`
- Build one project:
  - `dotnet build src/Backend/GameStateService/GameStateService.csproj`
  - `dotnet build src/Backend/GameService/GameService.csproj`

### Run Services

- Run GameStateService:
  - `dotnet run --project src/Backend/GameStateService/GameStateService.csproj`
- Run GameService:
  - `dotnet run --project src/Backend/GameService/GameService.csproj`
- Run GameNotificationService:
  - `dotnet run --project src/Backend/GameNotificationService/GameNotificationService.csproj`
- Run Frontend:
  - `dotnet run --project src/FrontEnd/TicTacToeMud/TicTacToeMud.csproj`
- Run Aspire AppHost:
  - `dotnet run --project src/TicTacToe.AppHost/TicTacToe.AppHost.csproj`

### Test (All / Project / Single Test)

- Run all tests in solution:
  - `dotnet test TicTacToe.sln`
- Run one test project:
  - `dotnet test tests/GameStateService.Tests/GameStateService.Tests.csproj`
  - `dotnet test tests/GameService.UnitTests/GameService.UnitTests.csproj`
  - `dotnet test tests/GameService.IntegrationTests/GameService.IntegrationTests.csproj`

- Run tests by class name (substring):
  - `dotnet test tests/GameStateService.Tests/GameStateService.Tests.csproj --filter "FullyQualifiedName~GameLogicMoveRequestHandlerUnitTests"`

- Run a single test method:
  - `dotnet test tests/GameStateService.Tests/GameStateService.Tests.csproj --filter "FullyQualifiedName~GameStateService.Tests.GameLogicMoveRequestHandlerUnitTests.HandleAsync_returns_success_for_valid_move_and_updates_board"`

- Run only integration tests (naming-based):
  - `dotnet test tests/GameStateService.Tests/GameStateService.Tests.csproj --filter "FullyQualifiedName~IntegrationTests"`

- Run only unit tests (naming-based):
  - `dotnet test tests/GameStateService.Tests/GameStateService.Tests.csproj --filter "FullyQualifiedName~UnitTests"`

### Formatting / Lint-like Checks

- Format code (if `dotnet format` is available):
  - `dotnet format TicTacToe.sln`
- Verify formatting without changes:
  - `dotnet format TicTacToe.sln --verify-no-changes`

## Test Environment Notes

- Integration tests use Testcontainers (PostgreSQL/RabbitMQ).
- Docker must be running for containerized integration tests.
- Some tests can be slower due to container startup.
- Prefer narrow test filters during iteration, then run full project tests.

## OpenSpec Workflow Commands (Used in This Repo)

- List active changes:
  - `openspec list --json`
- Check change status:
  - `openspec status --change "<name>" --json`
- Apply instructions for implementation:
  - `openspec instructions apply --change "<name>" --json`

## Code Style Guidelines

Follow existing code in touched files first; avoid broad style rewrites.

### C# Language and Project Defaults

- Target framework is `net10.0` in active projects.
- `Nullable` is enabled: write null-safe code and avoid nullable warnings.
- `ImplicitUsings` is enabled.
- Prefer file-scoped namespaces:
  - `namespace X.Y.Z;`

### Imports and File Layout

- Keep `using` statements at top of file.
- Order usings with framework/system first, then library/project namespaces.
- Avoid unused usings.
- Keep one primary type per file unless the file intentionally groups small related records/enums.

### Naming Conventions

- Types/methods/properties: PascalCase.
- Private fields: `_camelCase`.
- Local variables/parameters: camelCase.
- Async methods: suffix with `Async` (except framework overrides like `HandleAsync`).
- Test method names: descriptive, behavior-focused, often `Action_expected_outcome`.

### Types, Records, and Immutability

- Prefer `record`/`record class` for request/response DTO-style types.
- Use `required` init properties when creating contract objects.
- Use `sealed` where extension is not intended (common in handlers/mappers/tests).
- Prefer explicit return/result models over magic strings or tuple-heavy APIs.

### Endpoint and Handler Patterns

- FastEndpoints endpoints should focus on transport concerns only.
- Delegate business logic to `IRequestHandler<,>` handlers.
- Keep mapping logic in dedicated mapper classes where pattern exists.
- Use consistent response helpers:
  - `Send.OkAsync`, `Send.NotFoundAsync`, `Send.ErrorsAsync`.

### Error Handling and Validation

- Fail fast for invalid required arguments (e.g., null checks in mappers).
- Use validators (FluentValidation/FastEndpoints validators) for request validation.
- Return meaningful HTTP statuses; do not hide errors with generic 200 responses.
- Preserve public API compatibility unless change artifacts explicitly allow breaking changes.

### Dependency Injection

- Register dependencies in `Program.cs` / service extension modules.
- Keep lifetimes intentional:
  - Stateless components often `Scoped`.
  - In-memory repositories/singletons only where explicitly intended.
- Prefer constructor injection; avoid service locator patterns.

### Testing Conventions

- Unit tests:
  - Isolate logic with fakes/mocks/substitutes.
  - Assert both result and side effects for orchestrators.
- Integration tests:
  - Use fixture/base-class reuse to reduce startup cost.
  - Use deterministic wait/retry helpers for async messaging.
- Keep tests readable: Arrange / Act / Assert structure.

## Agent Working Rules

- Keep changes minimal and scoped to requested behavior.
- Do not rename/move unrelated files.
- Do not introduce new architectural patterns if an existing one already applies.
- Run the smallest relevant tests first, then broader tests before finishing.
- If a command fails due to environment (Docker, local secrets, external feeds), report clearly and continue with what can be verified.
