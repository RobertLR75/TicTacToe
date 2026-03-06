## Why

GameStateService endpoint logic is currently spread across endpoint classes and service calls, which makes request handling patterns inconsistent and harder to test in isolation. We should standardize command/query handling behind an `IRequest<T>` + `HandleAsync` pattern now to improve maintainability before more game workflows are added.

## What Changes

- Introduce a request handling contract in GameStateService based on `IRequest<TResponse>` and `HandleAsync` for feature-level request handlers.
- Refactor existing game-state operations (create game, get game, make move) to route through request handlers while preserving external HTTP behavior.
- Keep FastEndpoints endpoint contracts stable and delegate orchestration to handlers.
- Add or update tests to validate parity of behavior and guard refactor regressions.

## Capabilities

### New Capabilities
- `gamestate-request-handler-pattern`: Defines a consistent internal request/handler model for GameStateService operations using `IRequest<T>` and `HandleAsync`.

### Modified Capabilities
- `create-game`: Maintain create-game behavior while changing internal processing flow to request handlers.

## Impact

- Affected code: `src/Backend/GameStateService` endpoint classes, service-layer orchestration, and related tests.
- Public APIs: No intentional breaking API contract changes; request/response payloads and status codes should remain compatible.
- Dependencies/systems: No new external infrastructure required; MassTransit, MongoDB/Redis usage remains unchanged.
- Performance impact analysis: Additional handler indirection is in-process and expected to have negligible runtime overhead; monitor latency for create/get/move endpoints after rollout.
- Affected teams: Backend API and frontend consumer teams should be notified that behavior is unchanged but internals are being refactored.
- Rollback plan: Revert to the previous endpoint-to-service wiring by restoring pre-refactor endpoint implementations if regressions are observed.
