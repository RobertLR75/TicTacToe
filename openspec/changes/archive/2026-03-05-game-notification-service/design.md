## Context

The TicTacToe platform currently has two backend services (`GameStateService` and `GameApi`) and a Blazor frontend (`TicTacToeMud`), all orchestrated via .NET Aspire. As the platform grows, a dedicated notification service is needed to handle future concerns like game invitations, turn alerts, and match results. Rather than bolt notification logic onto an existing service later, this change scaffolds a blank `GameNotificationService` API now so that other services can plan integrations against a known service boundary.

The existing `GameStateService` project serves as the reference architecture: FastEndpoints for API routing, ServiceDefaults for OpenTelemetry/health checks/service discovery, and Aspire AppHost registration.

## Goals / Non-Goals

**Goals:**
- Scaffold a new ASP.NET Core project (`GameNotificationService`) that follows the same patterns as `GameStateService`.
- Register the project in the Aspire AppHost so it participates in orchestration, service discovery, and the Aspire dashboard.
- Include FastEndpoints, Swagger, ServiceDefaults, and a placeholder health endpoint.
- Keep the project minimal -- a blank slate ready for future feature development.

**Non-Goals:**
- Implementing actual notification logic (push, email, in-app, etc.).
- Adding database or message broker integrations (those will come with future features).
- Creating client libraries or SDKs for other services to consume.
- Defining a notification data model or persistence layer.

## Decisions

### 1. Project location: `src/Backend/GameNotificationService/`

**Rationale:** Follows the existing convention where backend services live under `src/Backend/`. Keeps the solution folder structure consistent (`src/Backend/` solution folder in `TicTacToe.sln`).

**Alternatives considered:**
- Top-level `src/GameNotificationService/` -- rejected because it breaks the Backend grouping convention.

### 2. Use FastEndpoints + Swagger (same as GameStateService)

**Rationale:** Consistency with the existing API style. FastEndpoints is already the established pattern, and Swagger provides immediate API documentation. Matching `GameStateService` means developers don't need to learn a different pattern.

**Alternatives considered:**
- Minimal APIs -- simpler but inconsistent with the existing codebase.
- Controllers -- heavier and not used anywhere in this project.

### 3. Register in Aspire AppHost with dedicated HTTP endpoint

**Rationale:** The service needs its own port for local development and should appear in the Aspire dashboard for observability. Following the pattern of `gamestateservice` (port 5110) and `gameapi` (port 5120), the notification service will use port 5130.

### 4. No resource references (Redis, RabbitMQ, etc.) initially

**Rationale:** This is a blank API. Wiring up infrastructure dependencies before there's logic to use them adds unnecessary complexity and slower startup. Resources will be added when notification features are implemented.

## Risks / Trade-offs

- **[Risk] Unused service consuming resources during local dev** -- Aspire will spin up an additional process. Mitigation: The service is minimal and lightweight; negligible resource impact.
- **[Trade-off] Port assignment (5130) may conflict** -- If another service is later added on the same port. Mitigation: Port is only for local dev and easily changed; Aspire service discovery abstracts this in non-local environments.
- **[Risk] Blank API could become a dumping ground** -- Without clear ownership, unrelated features could end up here. Mitigation: The proposal and spec clearly scope this to notifications; future changes should follow the same spec-driven process.
