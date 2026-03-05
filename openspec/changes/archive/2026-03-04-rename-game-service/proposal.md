## Why

The project `GameService` is named too generically -- it doesn't convey what the service manages. Renaming to `GameStateService` makes the project's responsibility explicit: it owns game state (creation, retrieval, and mutation via moves). This improves codebase navigability as additional services are added over time.

## What Changes

- **BREAKING**: Rename the `GameService` project directory from `src/Backend/GameService/` to `src/Backend/GameStateService/`
- **BREAKING**: Rename the project file from `GameService.csproj` to `GameStateService.csproj`
- **BREAKING**: Rename the root namespace from `GameService` to `GameStateService` across all source files
- Update the solution file (`TicTacToe.sln`) to reference the renamed project
- Update Aspire AppHost (`TicTacToe.AppHost`) project reference and orchestration registration
- Update all `using GameService.*` and `namespace GameService.*` statements in source files

## Capabilities

### New Capabilities

_None -- this is a pure rename/refactoring change with no new capabilities._

### Modified Capabilities

_None -- no spec-level behavior changes. All API contracts, endpoints, and behaviors remain identical._

## Impact

- **Source code**: All 14 `.cs` files under `src/Backend/GameService/` need namespace updates
- **Project files**: `GameService.csproj` renamed; `TicTacToe.AppHost.csproj` project reference updated
- **Solution file**: `TicTacToe.sln` project path and name updated
- **Aspire orchestration**: `AppHost.cs` resource name and project reference updated
- **APIs**: No changes -- all endpoints (`POST /api/games`, `GET /api/games/{id}`, `POST /api/games/{id}/moves`) remain identical
- **Dependencies**: No package or external dependency changes
- **Rollback plan**: Revert the rename commit. Since this is a single atomic rename with no behavioral changes, a git revert cleanly restores the prior state
- **Affected teams**: Any team or developer with local branches referencing files under `src/Backend/GameService/` will encounter merge conflicts on rebase
- **Performance impact**: None -- this is a compile-time rename with zero runtime effect
