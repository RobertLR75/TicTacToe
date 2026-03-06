## Why

`GameLogicService` currently centralizes winner and draw checks as service methods, which makes move resolution logic harder to compose and test as discrete request handlers. Refactoring these checks to `IRequest` handlers aligns game flow logic under the existing request pipeline and allows us to remove an unnecessary service abstraction.

## What Changes

- Refactor `GameLogicService.CheckWinner` into an `IRequest`-based handler placed under the `MakeMove` feature folder.
- Refactor `GameLogicService.CheckDraw` into an `IRequest`-based handler placed under the `MakeMove` feature folder.
- Update the move flow to call the new request handlers instead of `GameLogicService` methods.
- Delete `GameLogicService` and remove its registrations/usages after the new handlers are wired.
- Add or update unit tests around the new request handlers and move orchestration.

## Capabilities

### New Capabilities
- `move-outcome-evaluation-requests`: Evaluate winner and draw outcomes through request handlers used by the `MakeMove` flow.

### Modified Capabilities
- None.

## Impact

- Affected code: move-processing flow, `MakeMove` feature folder, DI registrations, and tests that depend on `GameLogicService`.
- Affected APIs: no external REST contract changes; behavior remains backward compatible.
- Dependencies/systems: no new infrastructure dependencies; uses existing request/handler pipeline.
- Affected teams: backend API team and QA automation team.
- Performance impact: expected neutral to minor improvement due to clearer orchestration and no extra service indirection.
- Rollback plan: restore `GameLogicService`, rewire `MakeMove` to call service methods, and keep request handlers disabled/removed if regressions are found.
