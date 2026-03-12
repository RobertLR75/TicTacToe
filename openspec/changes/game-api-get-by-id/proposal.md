## Why

The frontend and existing specs expect a `GET /api/games/{gameId}` API, but the current backend implementation does not provide a stable GameService get-by-id route for retrieving a single game's state. Formalizing this change now closes that gap, aligns the public API with current client expectations, and prevents further drift between contracts and implementation.

## What Changes

- Add a new GameService read capability for retrieving a single game by id through `GET /api/games/{gameId}`.
- Define the expected success and not-found behaviors, including the response payload shape used by the frontend.
- Document the service interaction needed for GameService to return current game state without changing existing create, list, or move APIs.
- Add implementation tasks covering endpoint, service integration, tests, and contract alignment.

## Capabilities

### New Capabilities
- `game-api-get-by-id`: Retrieve a single game's current state through the GameService public API.

### Modified Capabilities
- `frontend-api-client`: Clarify that `GetGameAsync(gameId)` reads game state through the GameService `GET /api/games/{gameId}` contract and preserves typed read behavior.

## Impact

- **Affected code**: `src/Backend/GameService`, shared service contracts, frontend typed API client, and related unit/integration tests.
- **Affected APIs**: adds public read route `GET /api/games/{gameId}` with non-breaking behavior.
- **Affected systems/teams**: backend API, frontend UI consuming game reads, and QA/integration test coverage.
- **Performance impact**: minimal; adds a single point-read path that should remain lightweight and avoid broad list-style queries.
- **Rollback plan**: remove or disable the new route and revert the frontend/client contract changes if deployment uncovers issues.
