## Context

GameService currently owns the public `/api/games` API surface for create and status operations, while GameStateService exposes the existing read endpoint for current board state at a different route (`/api/game-states/{gameId}`). The frontend typed client and current specs already assume a GameService-hosted `GET /api/games/{gameId}` route, so the design must bridge that mismatch without breaking existing clients or changing move orchestration.

This change touches multiple modules: GameService must expose a new read slice, shared contracts must stay aligned with frontend expectations, and tests must verify the route and payload contract. Stakeholders include frontend consumers, backend API maintainers, and test/QA workflows that validate the public API.

## Goals / Non-Goals

**Goals:**
- Add a backward-compatible `GET /api/games/{gameId}` endpoint on GameService.
- Return the same game-state payload shape already consumed by the frontend for single-game reads.
- Keep the endpoint thin by delegating orchestration to a handler or gateway abstraction.
- Verify success, not-found, and dependency behavior with unit and integration tests.

**Non-Goals:**
- Rework create-game, move, or update-status behavior.
- Replace GameStateService as the source of truth for current board state.
- Introduce new persistence stores, messaging flows, or caching layers.

## Decisions

### Decision: Add a dedicated GameService get-by-id vertical slice
GameService will add a new endpoint/handler pair under its existing Games feature structure so the public API remains feature-oriented and consistent with other GameService routes. This keeps transport concerns in FastEndpoints and business orchestration in a handler.

**Alternatives considered:**
- Repoint the frontend to call GameStateService directly: rejected because it preserves contract drift and avoids fixing the intended public API.
- Add route rewriting/proxy infrastructure outside GameService: rejected as unnecessary complexity for a single read route.

### Decision: Use a read gateway to obtain current state from GameStateService
The GameService handler should call GameStateService's existing read API or equivalent service abstraction to obtain live game state, then return the shared `GetGameResponse` contract. This avoids duplicating board-state storage in GameService and keeps GameStateService as the read authority for live state.

**Alternatives considered:**
- Persist full board state inside GameService and read locally: rejected because it duplicates ownership and expands consistency concerns.
- Return only persisted GameService metadata: rejected because the frontend requires full game-state details.

### Decision: Preserve response shape and HTTP semantics
The new route will return `200 OK` with `GetGameResponse` when the game exists, `404 Not Found` when the game is missing, and a transient failure status when the upstream state dependency is unavailable. This preserves typed frontend behavior and gives operators a clear failure mode.

**Alternatives considered:**
- Return a partial response when state is unavailable: rejected because it weakens the contract and complicates clients.
- Return `204 No Content` for missing games: rejected because existing patterns use `404 Not Found`.

## Risks / Trade-offs

- [Added synchronous dependency on GameStateService for reads] → Mitigation: keep the integration narrow, surface explicit failure codes, and cover availability behavior with tests.
- [Cross-service latency on game reads] → Mitigation: use a lightweight point-read call and avoid additional fan-out or transformation work.
- [Contract drift between GameService and GameStateService] → Mitigation: reuse shared request/response contracts and add integration coverage around the public route.
- [Rollback leaves frontend expecting unsupported route] → Mitigation: deploy route changes with test validation and revert client usage if rollback is required.

## Migration Plan

1. Add the GameService get-by-id slice and its state-read integration.
2. Register required dependencies and keep the existing GameStateService route intact during rollout.
3. Add or update tests for success, not-found, and dependency failure behavior.
4. Deploy GameService update before relying on the route in additional clients.
5. If issues occur, roll back by removing the new GameService read path and reverting any client dependency on it.

## Open Questions

- Should GameService call GameStateService through a typed `HttpClient` or an internal shared abstraction for local/test composition?
- What exact status code should GameService return when GameStateService is temporarily unavailable: `503 Service Unavailable` or direct propagation of upstream failure?
