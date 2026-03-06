## Context

GameStateService already started adopting `IRequest<TResponse>`-based handlers for endpoint orchestration, but game logic still relies on direct method calls in `GameLogicService`. This creates mixed patterns in a single service and makes game-rule orchestration less explicit in tests and dependency injection.

Constraints:
- Maintain existing API behavior and event publication semantics.
- Keep changes localized to GameStateService internals.
- Avoid introducing external mediator frameworks.

Stakeholders:
- Backend maintainers of GameStateService.
- Frontend consumers expecting unchanged gameplay behavior.

## Goals / Non-Goals

**Goals:**
- Introduce request-handler based execution for game logic operations.
- Keep game operation outcomes (success, validation failures, state updates) behaviorally identical.
- Improve testability of game logic orchestration paths.

**Non-Goals:**
- Changing game rules or board evaluation semantics.
- Modifying public endpoint routes or payload contracts.
- Adding cross-service/shared mediator infrastructure in this change.

## Decisions

1. Introduce dedicated request contracts for game logic actions.
   - Decision: Model game logic operations as explicit request/response types and process them via `HandleAsync` handlers.
   - Rationale: Makes each operation explicit and testable.
   - Alternative: Keep direct method calls on GameLogicService.
   - Why not chosen: Maintains inconsistent orchestration style.

2. Keep handlers colocated with the GameLogicService feature area.
   - Decision: Place request and handler classes near `GameLogicService` implementation.
   - Rationale: Improves discoverability and local ownership.
   - Alternative: Introduce a global handlers folder.
   - Why not chosen: Splits related logic across folders unnecessarily.

3. Use adapter-style transition for existing callers.
   - Decision: Update callers to invoke handlers through DI and remove direct method dependencies where practical.
   - Rationale: Keeps refactor incremental and low-risk.
   - Alternative: Big-bang rewrite of all gameplay flows.
   - Why not chosen: Higher regression risk and harder troubleshooting.

4. Preserve existing outcome mapping and event sequencing.
   - Decision: Keep existing status/outcome semantics and publication order unchanged.
   - Rationale: Protects compatibility for consumers and tests.
   - Alternative: Redefine response models while refactoring.
   - Why not chosen: Adds scope without user-facing value.

## Risks / Trade-offs

- [Risk] Additional abstractions may increase cognitive load for simple operations -> Mitigation: Keep request/response models minimal and names feature-focused.
- [Risk] Behavior drift during migration from direct method calls -> Mitigation: Add parity tests covering success/error paths and move outcomes.
- [Risk] Partial migration leaves mixed usage patterns -> Mitigation: Update all current game-logic invocation paths in the same change.

## Migration Plan

1. Add request/handler contracts for game logic operations.
2. Implement handlers and wire them with DI.
3. Refactor game operation call sites to use handlers.
4. Run unit and integration tests for gameplay and event behavior.
5. Deploy normally and monitor move endpoint error/latency signals.

Rollback strategy:
- Revert handler integration and restore direct `GameLogicService` method invocation.
- No schema/data migration involved; rollback is code-only.

## Open Questions

- Should a shared abstraction for logic-level request handlers be promoted to other backend services, or kept local until repeated patterns emerge?
- Do we want to follow this change with pipeline behaviors (for logging/validation) at the game-logic handler level?
