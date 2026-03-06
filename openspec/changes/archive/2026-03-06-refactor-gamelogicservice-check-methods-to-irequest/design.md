## Context

`GameLogicService` currently exposes `CheckWinner` and `CheckDraw` as imperative service methods. The `MakeMove` flow depends on this shared service, which couples move orchestration to a utility-style abstraction instead of the existing request/handler pattern already used across the codebase. The requested refactor moves outcome evaluation into request handlers under `MakeMove` and removes `GameLogicService` entirely while preserving external game behavior.

## Goals / Non-Goals

**Goals:**
- Move winner and draw evaluation into dedicated `IRequest` handlers located in the `MakeMove` feature folder.
- Keep public API behavior and response contract unchanged.
- Remove `GameLogicService` and all of its DI and call-site dependencies.
- Improve testability by isolating winner/draw logic into handler-focused units.

**Non-Goals:**
- Redesign move domain rules or board representation.
- Introduce new infrastructure dependencies, persistence changes, or API endpoints.
- Change game outcome semantics beyond parity with current behavior.

## Decisions

1. Use two explicit requests: one for winner evaluation and one for draw evaluation.
   - Rationale: maps 1:1 to current responsibilities and keeps handlers focused.
   - Alternative considered: a single combined outcome request. Rejected to avoid conflating winner and draw responsibilities and complicating tests.

2. Place handlers and request contracts under the `MakeMove` folder.
   - Rationale: keeps move-resolution logic co-located with its orchestration and reduces cross-feature navigation.
   - Alternative considered: create a shared `GameLogic` folder. Rejected because logic is specifically consumed by `MakeMove` and does not yet justify a broader module.

3. Update `MakeMove` orchestration to dispatch requests via mediator/pipeline instead of calling service methods.
   - Rationale: aligns with existing architectural pattern and enables uniform behaviors (logging, validation, tracing) through pipeline behaviors.
   - Alternative considered: static helper extraction. Rejected because it bypasses request pipeline conventions.

4. Preserve behavior parity through focused regression tests.
   - Rationale: refactor safety depends on proving same winner/draw outcomes across representative board states.
   - Alternative considered: rely only on existing integration tests. Rejected because they may not cover all edge-case board arrangements.

## Risks / Trade-offs

- [Risk] Request dispatch introduces minor overhead compared to direct service calls. → Mitigation: keep handlers lightweight; validate no meaningful latency regression in test runs.
- [Risk] Deleting `GameLogicService` may break hidden dependencies in tests or other features. → Mitigation: perform repo-wide usage sweep and update all call sites before removal.
- [Risk] Behavior drift during logic migration. → Mitigation: add parity tests for winner and draw edge cases before and after refactor.

## Migration Plan

1. Add request/handler types for winner and draw checks in `MakeMove`.
2. Update `MakeMove` handler/orchestrator to call the new requests.
3. Update tests to target request handlers and adjusted flow.
4. Remove `GameLogicService` implementation and DI registration after all usages are migrated.
5. Run unit and integration suites for move flow.

Rollback strategy:
- Reintroduce `GameLogicService` and restore prior `MakeMove` call path if parity or stability issues are detected.
- Keep refactor isolated so rollback is limited to game-logic wiring without API contract changes.

## Open Questions

- None currently; implementation can proceed with existing requirements.
