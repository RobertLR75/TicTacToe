## Context

`GameNotificationService` currently exposes API surface but does not consume domain events from the messaging layer. The platform already standardizes on RabbitMQ and MassTransit, and `GameStateService` is introducing/publishing game lifecycle events. This change adds the first event-consumer path in `GameNotificationService` while preserving backward compatibility and keeping runtime behavior low-risk by limiting handlers to `Console.WriteLine`.

## Goals / Non-Goals

**Goals:**
- Consume `GameCreated` and `GameStateUpdated` messages in `GameNotificationService` using MassTransit with RabbitMQ transport.
- Register consumers and receive endpoints in the service composition root with environment-driven configuration.
- Ensure message handling is intentionally minimal for now (`Console.WriteLine`) while validating message flow.
- Keep existing REST endpoints and public API behavior unchanged.

**Non-Goals:**
- Implement real notification persistence, delivery, or user-facing notification APIs.
- Introduce saga/workflow orchestration, custom retries, or dead-letter handling beyond defaults.
- Redesign event contracts or publishing behavior in upstream services.

## Decisions

- Use dedicated MassTransit consumers (`GameCreatedConsumer`, `GameStateUpdatedConsumer`) in `GameNotificationService`.
  - Rationale: keeps event handling isolated, testable, and aligned with MassTransit conventions.
  - Alternative considered: inline receive endpoint handlers in startup; rejected due to lower maintainability and weaker separation of concerns.
- Configure RabbitMQ via strongly typed options and register MassTransit at startup.
  - Rationale: consistent with service configuration patterns and supports safe environment-specific rollout.
  - Alternative considered: hardcoded broker details; rejected because it is operationally brittle.
- Keep consumer side effects to `Console.WriteLine` only for this phase.
  - Rationale: validates connectivity, routing, and deserialization without introducing domain risk.
  - Alternative considered: immediate persistence of notifications; rejected because requirements currently call for transport validation only.
- Use explicit queue/endpoint names for `GameCreated` and `GameStateUpdated` consumption in notification service context.
  - Rationale: improves traceability and avoids accidental coupling to generated names.
  - Alternative considered: rely entirely on default endpoint naming; rejected to reduce ambiguity during early rollout.

## Risks / Trade-offs

- [Consumers may fail to bind if RabbitMQ settings are incomplete] -> Mitigation: validate required configuration at startup and fail fast with clear errors.
- [At-least-once delivery may cause duplicate log output] -> Mitigation: accept duplicates for this phase and include identifiers in log text for diagnosis.
- [Console logging is low-fidelity for production observability] -> Mitigation: keep this as short-lived bootstrap behavior and follow with structured logging in a later change.
- [Queue naming mistakes can silently route messages elsewhere] -> Mitigation: define and document endpoint names explicitly in one configuration location.

## Migration Plan

1. Add MassTransit RabbitMQ registration to `GameNotificationService` startup with required configuration binding.
2. Add `GameCreated` and `GameStateUpdated` consumer classes that print message receipt details to console.
3. Bind consumers to explicit receive endpoints and validate startup in local/dev environments.
4. Deploy to non-production and verify event consumption using published test events.
5. Roll out to production with monitoring of startup health and consumer logs.
6. Rollback strategy: disable consumer endpoint registration/configuration or revert deployment to prior version.

## Open Questions

- Should event contracts be referenced from a shared contracts project or duplicated as local message types in this service?
- What final queue naming convention should be mandated across all consumer services (service-prefixed vs capability-prefixed)?
- When should this service transition from `Console.WriteLine` to structured Serilog + Application Insights telemetry?
