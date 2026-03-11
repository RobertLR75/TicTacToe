## 1. Test Infrastructure Foundation

- [x] 1.1 Create shared unit-test base classes for GameStateService handler/service test setup and common assertions.
- [x] 1.2 Create shared integration-test fixtures (xUnit collection fixtures) for Testcontainers lifecycle and dependency readiness checks.
- [x] 1.3 Create a reusable GameStateService integration host/client base class that standardizes bootstrapping and cleanup.
- [x] 1.4 Align `tests/GameStateService.Tests` project dependencies and configuration with the GameService test pattern.

## 2. Unit Test Coverage Expansion

- [x] 2.1 Add unit tests for all request handlers to cover success, failure, and edge branches with dependency interaction assertions.
- [x] 2.2 Add unit tests for supporting services and orchestration collaborators used by GameStateService runtime paths.
- [x] 2.3 Add/align endpoint parity unit tests to enforce endpoint contracts and handler delegation invariants.
- [x] 2.4 Refactor existing GameStateService unit tests to use shared base classes/fixtures and remove duplicated setup.

## 3. Integration Test Coverage Expansion

- [x] 3.1 Add Testcontainers-based integration tests for create/get/move runtime paths through GameStateService endpoints.
- [x] 3.2 Add integration tests that verify `GameCreatedEvent` publication behavior for successful and failed create flows.
- [x] 3.3 Add integration tests that verify `GameStateUpdatedEvent` publication behavior for successful and failed update flows.
- [x] 3.4 Ensure integration tests use deterministic wait/retry utilities and stable cleanup behavior to reduce flakiness.

## 4. Validation and CI Readiness

- [x] 4.1 Run `dotnet test tests/GameStateService.Tests/GameStateService.Tests.csproj` and fix failures until green.
- [x] 4.2 Validate containerized tests in CI-like conditions (Docker available, clean environment) and document assumptions.
- [x] 4.3 Confirm no public API contract changes in GameStateService endpoints while adding test coverage.
- [x] 4.4 Update test execution guidance (filters/grouping) for local and CI workflows if needed.
