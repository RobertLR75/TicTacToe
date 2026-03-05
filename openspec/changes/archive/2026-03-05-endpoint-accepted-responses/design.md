## Context

The TicTacToe application has two game endpoints in `GameStateService` — `CreateGameEndpoint` (`POST /api/games`) and `MakeMoveEndpoint` (`POST /api/games/{GameId}/moves`). Both currently return `200 OK` with the full game state in the response body. A SignalR notification system already exists: `NotificationService` has both `NotifyGameUpdated` and `NotifyGameCreated` methods, a `GameHub` manages per-game groups, and the frontend `GameHubClient` already listens for both `GameUpdated` and `GameCreated` events.

However, `CreateGameEndpoint` never calls `NotifyGameCreated`, and the client always reads state from the HTTP response rather than from SignalR. This means the notification infrastructure is underutilized, and the client is tightly coupled to synchronous HTTP responses for state delivery.

## Goals / Non-Goals

**Goals:**
- `MakeMoveEndpoint` returns `202 Accepted` with no body; client gets state from `GameUpdated` notification
- `CreateGameEndpoint` fires `GameCreated` notification with full game state and returns `202 Accepted` with `{ gameId }` only
- Frontend `Game.razor` uses `gameId` from the `202` to join the SignalR group, then receives full state via notifications
- `GameApiClient` return types simplified to match (string for create, void for move)
- `NotificationService.NotifyGameCreated` sends full game state (same shape as `GameUpdateNotification`)

**Non-Goals:**
- Changing the `GetGameEndpoint` — it remains `200 OK` with full state (query, not command)
- Adding message queue or async processing — the endpoints still process synchronously, just return `202`
- Multi-player matchmaking or lobby features
- Removing the `GameUpdated` notification from `MakeMoveEndpoint` (it stays)

## Decisions

### 1. Return `202 Accepted` instead of `200 OK`

**Decision**: Both command endpoints return `202 Accepted` to signal "request received, state will arrive via notification."

**Rationale**: `202` communicates that the action was accepted but the authoritative state is delivered asynchronously. This is semantically accurate since the client now relies on SignalR for state. `204 No Content` was considered for MakeMove but rejected because `202` better conveys the async-delivery pattern.

**Alternatives considered**:
- Keep `200 OK` with empty body — misleading; `200` implies the response body is the result
- `204 No Content` for MakeMove — acceptable but inconsistent with CreateGame which returns `{ gameId }`

### 2. CreateGame still returns `{ gameId }` in the 202 body

**Decision**: `CreateGameEndpoint` returns `{ gameId }` so the client can join the correct SignalR group before receiving the notification.

**Rationale**: The client needs the `gameId` to call `JoinGame(gameId)` on the hub. Without it, there's a race condition where the `GameCreated` notification fires before the client knows which group to join. Returning just the ID keeps the response minimal while solving the ordering problem.

### 3. NotifyGameCreated sends full game state

**Decision**: Update `NotifyGameCreated` to send a `GameCreatedNotification` with the same shape as `GameUpdateNotification` (all game state fields including board).

**Rationale**: The client needs full state to render the board. Sending only `{ gameId }` would force a follow-up `GET /api/games/{id}` request, adding latency and complexity. Using the same shape as `GameUpdateNotification` keeps the frontend mapping code consistent.

### 4. MakeMove returns no body

**Decision**: `MakeMoveEndpoint` returns `202 Accepted` with no response body. The `MakeMoveResponse` class is retained but unused (or removed).

**Rationale**: The `GameUpdated` notification already delivers the full game state to all clients in the game group. Returning state in the HTTP response is redundant. The client already has a `HandleGameUpdated` handler that calls `UpdateState` — it just needs to be the sole path for state updates after moves.

### 5. Frontend handles timing via optimistic UI pattern

**Decision**: After calling MakeMove, the frontend shows a loading indicator until the `GameUpdated` notification arrives. No optimistic local state mutation.

**Rationale**: The SignalR notification typically arrives within milliseconds since the endpoint sends it before returning `202`. Optimistic updates would add complexity and risk state inconsistency. The existing `_isLoading` flag and `MudProgressLinear` already handle the brief delay.

## Risks / Trade-offs

- **[SignalR disconnection]** → If SignalR is disconnected, the client won't receive state after a move. Mitigation: The frontend already shows a "Live" chip for connection status. Add a fallback `GetGameAsync` call if no notification arrives within a timeout (future enhancement, out of scope for this change).
- **[Race condition on CreateGame]** → Client must join the SignalR group before `NotifyGameCreated` fires. Mitigation: The endpoint calls `NotifyGameCreated` after persisting state. The client joins the group immediately after receiving the `202`. In practice the client's `JoinGame` completes before the notification is sent because it's the same server. If missed, the client can fall back to `GetGameAsync`.
- **[Breaking API change]** → External consumers of `POST /api/games` and `POST /api/games/{id}/moves` will break. Mitigation: Currently no known external consumers — the only client is the Blazor frontend. Document the change in API docs.
- **[Test changes required]** → Existing endpoint tests that assert on response body will need updating. Mitigation: Straightforward — change assertions to check status code `202` and verify notification was sent.
