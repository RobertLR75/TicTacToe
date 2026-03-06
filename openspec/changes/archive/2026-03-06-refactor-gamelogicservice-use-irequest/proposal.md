## Why

`GameLogicService` currently exposes imperative methods directly, which makes request orchestration conventions inconsistent with the new request-handler direction in `GameStateService`. Standardizing `GameLogicService` usage around `IRequest<T>` and `HandleAsync` now keeps feature flows coherent and easier to test as game rules evolve.

## What Changes

- Refactor `GameLogicService` interactions to use request/handler abstractions (`IRequest<TResponse>` + `HandleAsync`) for game-logic operations.
- Introduce dedicated request models and handlers for key game-logic actions used by move processing.
- Update dependent orchestration paths to consume handlers instead of calling logic methods directly.
- Add or update tests to validate behavior parity and ensure no gameplay regressions.

## Capabilities

### New Capabilities
- `gamelogic-request-handlers`: Defines request/handler-based execution for game-logic operations with explicit request models and `HandleAsync` processing.

### Modified Capabilities
- `game-state-events`: Preserve existing game state update behavior and event publication semantics while changing internal game-logic invocation flow.

## Impact

- Affected code: `src/Backend/GameStateService/Services/GameLogicService.cs`, related endpoint/handler orchestration, and `tests/GameStateService.Tests`.
- Public APIs: No intentional changes to endpoint routes, payload schemas, or status code contracts.
- Dependencies/systems: No new infrastructure or external library dependencies required.
- Performance impact analysis: Additional in-process handler dispatch introduces negligible overhead; move endpoint latency should be monitored after rollout.
- Affected teams: Backend service maintainers and frontend consumers relying on stable move/update behavior.
- Rollback plan: Revert `GameLogicService` request-handler integration and restore previous direct method invocation path if regressions are observed.
