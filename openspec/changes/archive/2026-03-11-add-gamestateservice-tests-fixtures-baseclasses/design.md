## Context

GameStateService already contains handler and endpoint logic with some tests, but test setup is fragmented and does not follow a reusable, layered structure. Recent GameService work established a clear split between unit tests (fast, isolated) and integration tests (Testcontainers-backed), including reusable fixture/base abstractions. We need the same architecture in GameStateService to reduce regression risk and ensure future changes can be validated consistently in local and CI runs.

Constraints:
- Keep public API and endpoint contracts backward compatible.
- Keep production runtime behavior unchanged unless minor testability adjustments are required.
- Ensure integration tests can run in CI environments with Docker/Testcontainers.

Stakeholders:
- Backend service developers (primary)
- QA/automation and CI maintainers (secondary)

## Goals / Non-Goals

**Goals:**
- Establish a standardized GameStateService test foundation using reusable fixtures and base classes.
- Provide comprehensive unit coverage for handlers, services, validators/contract parity, and branching logic.
- Provide integration coverage for runtime-relevant paths (endpoint wiring and event publishing) using Testcontainers.
- Mirror the organizational and naming pattern used in `GameService.Tests` for consistency and maintainability.

**Non-Goals:**
- Rewriting GameStateService business rules or endpoint contracts.
- Introducing new product features.
- Replacing existing non-GameStateService tests in other projects.

## Decisions

1. **Adopt the same layered test pattern as GameService.Tests**
   - Decision: Structure tests into unit-focused and integration-focused files under `tests/GameStateService.Tests`, with shared fixture/base abstractions.
   - Rationale: This aligns team conventions, reduces onboarding friction, and improves reuse.
   - Alternatives considered:
     - Keep ad-hoc per-test setup: rejected due to duplication and drift.
     - Split into separate projects (`.UnitTests` and `.IntegrationTests`): deferred to avoid project sprawl right now.

2. **Use xUnit fixtures/collections for shared integration setup**
   - Decision: Introduce fixture classes (e.g., Testcontainers lifecycle, host/client bootstrapping) and collection-based sharing to avoid repeated container startup.
   - Rationale: Faster test execution and deterministic setup/teardown.
   - Alternatives considered:
     - Per-test container startup: rejected for performance.
     - Static singleton bootstrap without xUnit fixture control: rejected for lifecycle brittleness.

3. **Keep unit tests isolated with mocks/fakes and deterministic assertions**
   - Decision: For handlers/services, use mocked dependencies/fakes and assert both outputs and side effects (publish calls, repository updates, error branches).
   - Rationale: Maximizes speed and debuggability while preserving behavior coverage.
   - Alternatives considered:
     - Hitting real infrastructure in most tests: rejected due to slowness and nondeterminism.

4. **Scope integration tests to critical runtime seams**
   - Decision: Validate key endpoint and event-flow paths against containerized dependencies rather than exhaustive combinatorics.
   - Rationale: Balances confidence and CI runtime.
   - Alternatives considered:
     - Exhaustive end-to-end permutations in integration layer: rejected; push permutation coverage into unit tests.

## Risks / Trade-offs

- **[Risk] CI runtime increase from containerized tests** → Mitigation: reuse fixture instances per test collection and keep integration scenarios focused.
- **[Risk] Flaky integration tests from environment variance** → Mitigation: deterministic waits/timeouts, robust cleanup, and explicit dependency readiness checks.
- **[Risk] Over-coupling tests to implementation details** → Mitigation: prioritize behavior/contract assertions; limit reflection/internal-structure assertions to endpoint parity checks.
- **[Trade-off] Added abstraction (fixtures/base classes) increases upfront complexity** → Mitigation: document conventions in test code and keep base classes minimal.

## Migration Plan

1. Add shared test infrastructure (fixtures/base classes) in `tests/GameStateService.Tests`.
2. Refactor/align existing GameStateService tests to consume shared infrastructure.
3. Add missing unit tests for all handlers/services/endpoints.
4. Add missing integration tests using Testcontainers for runtime-critical paths.
5. Run full `dotnet test` for GameStateService tests and stabilize flaky cases.
6. Enable/confirm CI execution and test filters.

Rollback strategy:
- Revert new test files and project test dependency changes.
- Disable new integration tests via test filters if CI instability appears.
- Keep safe, deterministic unit tests even if containerized layer is temporarily reduced.

## Open Questions

- Should integration tests run on every PR or only on main/nightly for faster feedback?
- Do we want a single shared cross-service Testcontainers utility package later, or keep per-service fixtures for now?
