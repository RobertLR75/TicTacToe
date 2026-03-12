## Context

The AppHost currently defines infrastructure (Redis, MongoDB, PostgreSQL, RabbitMQ) and registers the backend and frontend projects, but orchestration expectations are documented primarily around frontend startup with GameService. As additional services become part of the local runtime, explicit orchestration behavior is needed so GameStateService and GameNotificationService are consistently managed with correct references and startup dependencies.

This is a platform-level change touching service orchestration, resource dependency wiring, and local developer experience in Aspire. Stakeholders include platform maintainers of AppHost plus backend/frontend developers who run the distributed app locally.

## Goals / Non-Goals

**Goals:**
- Ensure AppHost explicitly orchestrates GameStateService and GameNotificationService as managed resources.
- Define required resource references (databases, message broker, cache) and startup ordering in a stable, testable way.
- Keep frontend service references aligned with the orchestrated backend services used for gameplay and notifications.
- Preserve current environment/secrets handling patterns (Aspire parameters and service references) without introducing hard-coded credentials.

**Non-Goals:**
- Redesigning backend service internals or API contracts.
- Replacing existing infrastructure technologies (RabbitMQ, PostgreSQL, Redis, etc.).
- Introducing production deployment pipeline changes in this spec; scope is AppHost orchestration behavior.

## Decisions

### Decision: Treat GameStateService and GameNotificationService as first-class orchestrated projects
- **Choice:** Maintain explicit `AddProject` registrations for both services in AppHost and define named HTTP endpoints for clear service discovery.
- **Rationale:** Explicit registration keeps Aspire dashboard visibility and local startup lifecycle deterministic.
- **Alternatives considered:**
  - Start services outside AppHost manually: rejected because it fragments orchestration and observability.
  - Use implicit defaults with no endpoint names: rejected because explicit naming improves consistency and diagnosability.

### Decision: Standardize resource references and waits per service responsibility
- **Choice:** GameStateService references cache, persistence, and broker resources it depends on; GameNotificationService references broker and persistence resources and waits for critical dependencies before starting.
- **Rationale:** Resource wiring should mirror runtime dependencies so startup failures are surfaced early and reproducibly.
- **Alternatives considered:**
  - Minimal references with no startup waits: rejected due to flaky startup ordering.
  - Over-broad references to all resources for every service: rejected to avoid unnecessary coupling.

### Decision: Keep frontend references aligned to orchestrated gameplay and notification services
- **Choice:** Frontend keeps references to the services it calls/consumes during interactive sessions, including notification service.
- **Rationale:** This preserves local service discovery and telemetry correlation across UI-to-service interactions.
- **Alternatives considered:**
  - Frontend references only one backend and bypasses notification service discovery: rejected as incomplete for real-time flow.

## Risks / Trade-offs

- [Over-specified AppHost wiring becomes stale as services evolve] → Mitigation: Keep specs and AppHost updates synchronized via change-based workflow.
- [Incorrect dependency waits can delay startup or mask issues] → Mitigation: Only wait on true hard dependencies and validate with Aspire run checks.
- [Service discovery alias mismatch across projects] → Mitigation: Use stable resource names/endpoints and verify callers reference the same names.

## Migration Plan

1. Update AppHost requirements/spec to include GameStateService and GameNotificationService orchestration behavior.
2. Adjust `src/TicTacToe.AppHost/AppHost.cs` registration/reference/wait configuration to match spec requirements.
3. Validate distributed startup via AppHost run and ensure all resources/services become healthy.
4. Confirm frontend can resolve referenced services during local execution.

Rollback strategy:
- Revert AppHost orchestration changes for affected service registrations/references.
- No data migration rollback required because the change is orchestration-only.

## Open Questions

- Should AppHost include explicit health-gated waits for GameStateService before frontend start, or remain reference-only for faster startup during development?
