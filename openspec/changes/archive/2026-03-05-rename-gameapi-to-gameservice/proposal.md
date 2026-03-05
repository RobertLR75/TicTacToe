## Why

The `GameApi` project name is inconsistent with the codebase naming convention established by `GameStateService`. Renaming it to `GameService` aligns the lobby/matchmaking backend with the same `*Service` pattern, making the architecture clearer: `GameService` handles game lobby lifecycle while `GameStateService` handles in-game state and logic.

## What Changes

- Rename the `GameApi` project directory, `.csproj` file, and all internal namespaces from `GameApi.*` to `GameService.*`
- Update the solution file (`TicTacToe.sln`) project reference to point to the renamed project
- Update the Aspire AppHost project reference and resource name from `"gameapi"` to `"gameservice"`
- Update the AppHost endpoint name from `"gameapi-http"` to `"gameservice-http"`
- All API endpoint paths (`/api/game-lobby/*`) and runtime behavior remain unchanged — this is a code-level rename only

## Capabilities

### New Capabilities

_None — this is a rename/refactor with no new functionality._

### Modified Capabilities

- `rename-project`: Expanding the project naming convention to cover the `GameApi` project in addition to `GameStateService`

## Impact

- **Code**: 14 source files across the `GameApi` project require namespace updates (28 namespace/using statements). The AppHost project requires reference and resource name updates.
- **Solution**: `TicTacToe.sln` project entry must be updated with the new name and path.
- **APIs**: No breaking changes — all HTTP endpoint paths remain at `/api/game-lobby/*`.
- **Dependencies**: No package or external dependency changes. Redis key patterns (`game:{id}`) are unaffected.
- **Aspire service discovery**: The resource name changes from `"gameapi"` to `"gameservice"`. Currently no other services reference this name for discovery, so there is no downstream impact.
- **Rollback plan**: Revert the directory rename, restore the original `.sln` and AppHost references. Since this is a pure rename with no behavioral changes, rollback is a straightforward git revert.
- **Affected teams**: Any team working on the lobby/matchmaking service will see updated project and namespace names.
- **Performance impact**: None — this is a compile-time rename with no runtime changes.
