## Context

The frontend currently uses a single typed client, `GameApiClient`, for both GameService operations (`CreateGameAsync`, `ListGamesAsync`) and GameStateService operations (`GetGameAsync`, `MakeMoveAsync`). The backend routes already separate these concerns: create/list live under GameService, while game-state reads and move submission live under GameStateService. The change spans DI registration, frontend gameplay flow wiring, and tests, so documenting the boundary now reduces ambiguity before implementation.

## Goals / Non-Goals

**Goals:**
- Introduce a dedicated typed `GameStateServiceClient` for game-state reads and move submission.
- Keep `GameApiClient` focused on GameService-backed operations such as creating and listing games.
- Preserve current gameplay UX, SignalR updates, and existing backend API contracts.
- Keep rollback simple by changing only frontend client boundaries, not endpoint shapes.

**Non-Goals:**
- Changing GameService or GameStateService endpoint contracts.
- Reworking the page flow, SignalR hub behavior, or game-state domain models.
- Merging the two backend services behind a new gateway or abstraction layer.

## Decisions

1. **Add a second typed `HttpClient` service for GameStateService**
   - Create `GameStateServiceClient` in `src/FrontEnd/TicTacToeMud/Services` with `GetGameAsync` and `MakeMoveAsync` methods.
   - Register it in `src/FrontEnd/TicTacToeMud/Program.cs` with the same GameStateService base address pattern already used by the frontend.
   - Rationale: this matches the actual backend ownership and keeps UI components on typed clients rather than direct `HttpClient` use.
   - Alternative considered: keep all methods in `GameApiClient`. Rejected because the name and responsibility no longer match the endpoints it calls.

2. **Move only gameplay state operations to the new client**
   - `CreateGameAsync` and `ListGamesAsync` remain on `GameApiClient` because they target GameService and are already used by the home page/game list flow.
   - `GetGameAsync` and `MakeMoveAsync` move to `GameStateServiceClient`, and gameplay components inject both clients where needed.
   - Rationale: this produces a clean service boundary with minimal surface-area change.
   - Alternative considered: rename `GameApiClient` and move all methods at once. Rejected because it would expand the refactor and create unnecessary churn in already-correct GameService flows.

3. **Update tests to reflect the new client ownership rather than changing behavior assertions**
   - UI/component tests should stub `GameStateServiceClient` for state reads and move submission while continuing to stub `GameApiClient` for list/create paths.
   - Rationale: this preserves current behavior coverage while aligning tests with the intended architecture.
   - Alternative considered: defer tests and rely on manual verification. Rejected because the change is mainly about wiring and ownership, which tests can validate cheaply.

## Risks / Trade-offs

- [Risk] Dual client registration can be misconfigured to different base addresses. -> Mitigation: register both clients explicitly in `Program.cs` using the same local/Aspire service-discovery convention where appropriate.
- [Risk] UI code may keep stale references to moved methods on `GameApiClient`. -> Mitigation: remove or relocate the moved methods so compiler errors reveal missed usage sites.
- [Risk] Test fixtures may only register `GameApiClient` today. -> Mitigation: update affected tests to register both typed clients or focused stubs before implementation is considered complete.
- [Trade-off] The frontend will inject two API clients in gameplay flows instead of one. -> Mitigation: accept the extra dependency because it makes backend ownership clearer and scales better for future service-specific features.

## Migration Plan

1. Add `GameStateServiceClient` with the game-state GET and move POST operations.
2. Register the new typed client in `src/FrontEnd/TicTacToeMud/Program.cs`.
3. Update gameplay components and tests to use the new client for state-specific calls.
4. Run focused frontend tests, then broader solution validation as needed.
5. If regressions appear, roll back by reintroducing the game-state calls on `GameApiClient` and reverting the injection changes.

## Open Questions

- None. The existing backend routes and frontend usage sites provide enough context to implement the split without additional API design decisions.
