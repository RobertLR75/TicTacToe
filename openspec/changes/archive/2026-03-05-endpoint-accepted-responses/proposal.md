## Why

Both `CreateGameEndpoint` and `MakeMoveEndpoint` currently return `200 OK` with full game state synchronously. This couples clients to the HTTP response for state and prevents a clean event-driven flow. By switching to `202 Accepted` and having the client receive state via SignalR notifications, we decouple command acknowledgment from state delivery — enabling future scalability (async processing, multiple listeners) and activating the already-implemented but unused `NotifyGameCreated` path.

## What Changes

- **MakeMoveEndpoint** returns `202 Accepted` (no body) instead of `200 OK` with game state. Clients rely on the existing `GameUpdated` SignalR notification for state updates.
- **CreateGameEndpoint** injects `NotificationService` and calls `NotifyGameCreated` after creating the game, then returns `202 Accepted` with only `{ gameId }` instead of `200 OK` with full state.
- **Frontend Game.razor** updated so `StartNewGame` uses the `gameId` from the `202` response, joins the SignalR group, and waits for the `GameCreated` notification to populate initial state.
- **Frontend Game.razor** updated so `OnCellClick` no longer reads state from the HTTP response; it relies on the existing `GameUpdated` SignalR handler.
- **GameApiClient** updated: `CreateGameAsync` returns only `string` (gameId), `MakeMoveAsync` returns `void` (or just ensures success).
- **GameHubClient** `OnGameCreated` event updated to deliver full `GameResponse` instead of just `gameId`, matching the `GameCreated` notification payload.
- **NotificationService.NotifyGameCreated** updated to send full game state (matching `GameUpdateNotification` shape) instead of just `{ GameId }`.

## Capabilities

### New Capabilities

_None — this change modifies existing capabilities only._

### Modified Capabilities

- `create-game`: `POST /api/games` changes from returning `200 OK` with full game state to `202 Accepted` with `{ gameId }` only. Game state delivered via `GameCreated` SignalR notification.
- `game-page`: Game page creation flow changes to receive initial state from `GameCreated` notification rather than HTTP response. Move flow changes to rely on `GameUpdated` notification rather than HTTP response.
- `frontend-api-client`: `CreateGameAsync` return type changes from `GameResponse` to `string`. `MakeMoveAsync` return type changes from `GameResponse` to `void`.

## Impact

- **Backend**: `CreateGameEndpoint.cs`, `MakeMoveEndpoint.cs`, `NotificationService.cs`, `CreateGameResponse.cs`, `MakeMoveResponse.cs`
- **Frontend**: `GameApiClient.cs`, `GameHubClient.cs`, `Game.razor`, `GameResponse.cs`
- **Breaking change for direct API consumers**: Both endpoints change response status codes and body shapes. Any external client calling these endpoints directly will break.
- **Rollback plan**: Revert endpoint responses to `200 OK` with full state, remove `NotificationService` from `CreateGameEndpoint`, restore `GameApiClient` return types. All changes are in-memory state only (no schema migrations).
- **Affected teams**: Frontend, Backend
- **Performance impact**: Marginal — removes redundant state from HTTP responses, but adds reliance on SignalR for all state delivery. Net neutral for single-client scenarios; beneficial for multi-client scenarios.
