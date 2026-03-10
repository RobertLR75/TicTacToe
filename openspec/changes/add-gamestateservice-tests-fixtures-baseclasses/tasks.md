## 1. Test Infrastructure Foundation

- [ ] 1.1 Create shared unit-test base classes for GameStateService handler/service test setup and common assertions.
- [ ] 1.2 Create shared integration-test fixtures (xUnit collection fixtures) for Testcontainers lifecycle and dependency readiness checks.
- [ ] 1.3 Create a reusable GameStateService integration host/client base class that standardizes bootstrapping and cleanup.
- [ ] 1.4 Align `tests/GameStateService.Tests` project dependencies and configuration with the GameService test pattern.

## 2. Unit Test Coverage Expansion

- [ ] 2.1 Add unit tests for all request handlers to cover success, failure, and edge branches with dependency interaction assertions.
- [ ] 2.2 Add unit tests for supporting services and orchestration collaborators used by GameStateService runtime paths.
- [ ] 2.3 Add/align endpoint parity unit tests to enforce endpoint contracts and handler delegation invariants.
- [ ] 2.4 Refactor existing GameStateService unit tests to use shared base classes/fixtures and remove duplicated setup.

## 3. Integration Test Coverage Expansion

- [ ] 3.1 Add Testcontainers-based integration tests for create/get/move runtime paths through GameStateService endpoints.
- [ ] 3.2 Add integration tests that verify `GameCreatedEvent` publication behavior for successful and failed create flows.
- [ ] 3.3 Add integration tests that verify `GameStateUpdatedEvent` publication behavior for successful and failed update flows.
- [ ] 3.4 Ensure integration tests use deterministic wait/retry utilities and stable cleanup behavior to reduce flakiness.

## 4. Validation and CI Readiness

- [ ] 4.1 Run `dotnet test tests/GameStateService.Tests/GameStateService.Tests.csproj` and fix failures until green.
- [ ] 4.2 Validate containerized tests in CI-like conditions (Docker available, clean environment) and document assumptions.
- [ ] 4.3 Confirm no public API contract changes in GameStateService endpoints while adding test coverage.
- [ ] 4.4 Update test execution guidance (filters/grouping) for local and CI workflows if needed.
