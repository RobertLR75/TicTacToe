## Why

The frontend currently routes both GameService and GameStateService calls through `GameApiClient`, even though game-state reads and move submission target a different backend with different routes. Creating a dedicated `GameStateServiceClient` clarifies service ownership, reduces accidental endpoint drift, and makes future game-state features safer to evolve.

## What Changes

- Add a dedicated typed `GameStateServiceClient` for reading game state and submitting moves to GameStateService endpoints.
- Update frontend game-play flows to use `GameStateServiceClient` for game-state operations while keeping `GameApiClient` focused on GameService operations such as create/list.
- Preserve existing user-facing gameplay behavior while making backend client responsibilities explicit.
- Document a rollback path by retaining the existing `GameApiClient` call patterns until the replacement is verified and can be safely reverted if issues appear.

## Capabilities

### New Capabilities
- `frontend-game-state-client`: Defines frontend behavior for calling game-state read and move endpoints through a dedicated typed client.

### Modified Capabilities

## Impact

- Affected code: `src/FrontEnd/TicTacToeMud/Services`, frontend pages/components that load a game or submit moves, and related UI tests.
- APIs/systems: Frontend typed `HttpClient` registrations continue to call the same REST endpoints, but with clearer separation between GameService and GameStateService.
- Affected teams: frontend and backend contributors working on gameplay flows and service contracts.
- Performance impact: negligible at runtime because this is primarily a client-boundary change; startup DI registration increases only by one typed client.
- Rollback plan: revert frontend registrations and usage sites back to `GameApiClient` if integration or UI verification shows regressions.
