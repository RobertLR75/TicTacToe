## Context

GameNotificationService currently calls `ApplyNotificationMigrations()` during application startup, and that path executes FluentMigrator against PostgreSQL immediately. If PostgreSQL is not reachable yet, the application throws an unhandled `NpgsqlException` and terminates before the service can expose health endpoints, logs, or any useful operational signal. This is especially brittle for local development and Aspire-managed startup, where infrastructure readiness and process readiness can briefly diverge.

The change is limited to startup, persistence initialization, and operational behavior in `src/Backend/GameNotificationService` plus related tests. The existing notification endpoint contract and underlying notification storage model should remain intact. Stakeholders include backend developers, local orchestration maintainers, and frontend consumers that benefit from a more diagnosable service lifecycle.

## Goals / Non-Goals

**Goals:**
- Prevent an unhandled process crash when PostgreSQL is temporarily unavailable during GameNotificationService startup.
- Keep database migration execution explicit and deterministic once PostgreSQL becomes reachable.
- Surface persistence availability through structured diagnostics and health/readiness behavior.
- Preserve current endpoint contracts and normal persistence behavior when the database is available.
- Keep the design testable through focused unit and integration coverage.

**Non-Goals:**
- Changing the notification API shape or introducing new endpoints.
- Replacing PostgreSQL, FluentMigrator, or the current repository abstraction.
- Adding a general-purpose background job framework or broad retry infrastructure for other services.
- Making the service fully functional without PostgreSQL; persistence-backed operations may still report dependency unavailability until the database is ready.

## Decisions

1. **Move migration execution behind a controlled startup service instead of unconditional boot-time execution**
   - **Decision:** Replace the direct `app.Services.ApplyNotificationMigrations()` startup call with a dedicated initialization path that catches connection failures, logs them clearly, and records current persistence readiness state.
   - **Rationale:** This keeps migration behavior explicit while preventing an immediate unhandled exception from terminating the process.
   - **Alternative considered:** Keep the direct startup migration call and rely solely on Aspire `WaitFor(postgres)` sequencing.
   - **Why not alternative:** `WaitFor` improves orchestration sequencing but does not guarantee database reachability in every local/test/runtime scenario, so the crash path still exists.

2. **Represent persistence availability as a service-level readiness state**
   - **Decision:** Introduce a small readiness component for notification persistence that can be updated by initialization logic and consumed by health checks and endpoints.
   - **Rationale:** A single readiness source keeps behavior consistent across diagnostics, health reporting, and request handling.
   - **Alternative considered:** Let each endpoint or repository call handle connection failures independently.
   - **Why not alternative:** That spreads startup concerns across features, duplicates logic, and makes system state harder to reason about.

3. **Fail requests gracefully while persistence is unavailable**
   - **Decision:** When notification persistence has not been initialized successfully, notification API requests should return a clear service-unavailable outcome instead of surfacing low-level database exceptions.
   - **Rationale:** This preserves API stability and gives callers an actionable response during transient startup issues.
   - **Alternative considered:** Return empty results until the database is ready.
   - **Why not alternative:** An empty result would incorrectly imply a successful query and hide an operational dependency failure.

4. **Retry or re-attempt initialization within the service process rather than requiring manual restart**
   - **Decision:** Use a bounded/background reattempt strategy for notification persistence initialization so the service can recover once PostgreSQL becomes reachable.
   - **Rationale:** This is more resilient for local orchestration and reduces the need for manual intervention after transient database startup delays.
   - **Alternative considered:** Require operators/developers to restart the service after PostgreSQL is available.
   - **Why not alternative:** It leads to poor local developer ergonomics and unnecessary operational friction for transient startup races.

5. **Add focused tests around unavailable-database startup and recovery behavior**
   - **Decision:** Cover initialization failure handling, readiness transitions, health/reporting behavior, and request handling under dependency-unavailable conditions with targeted unit and integration tests.
   - **Rationale:** Startup resilience is easy to regress without tests because it spans boot logic, persistence, and HTTP behavior.
   - **Alternative considered:** Limit verification to manual Aspire startup testing.
   - **Why not alternative:** Manual validation alone is slow and unreliable for regression prevention.

## Risks / Trade-offs

- **[Risk] Background retry logic may delay visibility of a persistent database outage** → **Mitigation:** Emit structured logs on each failure and expose unhealthy readiness until initialization succeeds.**
- **[Risk] Returning service-unavailable responses changes failure mode for existing callers** → **Mitigation:** Keep the HTTP contract intact and use explicit dependency-unavailable messaging only when persistence cannot serve requests.**
- **[Trade-off] Startup resilience adds stateful initialization logic to a previously simple boot path** → **Mitigation:** Encapsulate the logic in a dedicated initializer/readiness component with narrow tests.**
- **[Trade-off] Migrations may occur slightly later than process start** → **Mitigation:** Gate readiness and persistence-backed request handling on successful initialization rather than assuming immediate availability.**

## Migration Plan

1. Introduce a notification persistence initializer and readiness state abstraction in GameNotificationService.
2. Replace direct startup migration execution with the initializer path and integrate it with health/readiness checks.
3. Update notification endpoints and/or repository-facing code to return a controlled dependency-unavailable response while persistence is not ready.
4. Add or update unit and integration tests for startup failure, recovery, and request behavior.
5. Verify local/Aspire startup when PostgreSQL is delayed or unavailable, then verify normal startup when PostgreSQL is healthy.

Rollback:
- Revert the initializer/readiness changes.
- Restore the direct startup migration execution path.
- Remove dependency-unavailable request handling and associated tests if the resilience approach proves too complex or incompatible.

## Open Questions

- Should dependency-unavailable behavior be exposed through standard ASP.NET Core health checks only, or also through a dedicated readiness endpoint/status payload?
- What retry interval and timeout budget are appropriate for local development versus automated test execution?
- Should consumer message processing also be gated on notification persistence readiness, or is HTTP request gating sufficient for this change?
