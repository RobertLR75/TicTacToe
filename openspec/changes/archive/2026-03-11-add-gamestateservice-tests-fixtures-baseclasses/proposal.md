## Why

GameStateService has partial automated test coverage, but it lacks a complete, reusable testing structure aligned with the new GameService testing approach. This increases regression risk when changing handlers, endpoint wiring, event publishing behavior, and infrastructure integration.

## What Changes

- Add a complete GameStateService unit test suite for handlers, core services, and endpoint contract parity.
- Add GameStateService integration tests using Testcontainers for external dependencies used in runtime paths.
- Introduce shared fixtures and base test classes to standardize setup, teardown, and dependency bootstrapping across GameStateService tests.
- Align naming, directory structure, and test layering with the established `GameService.Tests` unit/integration pattern.
- Ensure test coverage includes happy paths, failure paths, and dependency interaction assertions.

## Capabilities

### New Capabilities
- `gamestate-testing-foundation`: Standardized GameStateService unit and integration testing architecture with reusable fixtures and base classes.

### Modified Capabilities
- `gamestate-request-handler-pattern`: Add explicit testability and coverage requirements for request handlers and endpoint-to-handler wiring.
- `game-state-events`: Add integration coverage requirements for event publishing behavior under containerized dependencies.

## Impact

- Affected code:
  - `tests/GameStateService.Tests/**/*`
  - `src/Backend/GameStateService/**/*` (minimal runtime changes only if needed for testability)
- Affected teams:
  - Backend service team (GameStateService ownership)
  - QA/automation team (test execution and reliability)
  - CI maintainers (pipeline runtime and containerized test execution)
- Dependencies/systems:
  - xUnit test project configuration
  - Testcontainers dependencies and Docker runtime in CI
  - Messaging/test infrastructure configuration used by GameStateService integration tests
- Performance impact analysis:
  - No production runtime performance impact expected.
  - CI/test execution time will increase due to container startup and integration test execution.
  - Mitigation: consolidate fixtures, reuse containers per test collection, and keep high-volume permutations in unit tests.
- Rollback plan:
  - Revert new test files and test project configuration changes for GameStateService.
  - Remove newly added fixtures/base classes if they destabilize CI.
  - Temporarily gate containerized tests behind existing CI filters while retaining unit tests.
