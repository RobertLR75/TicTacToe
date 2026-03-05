## Context

`GameStateService` currently creates and updates game state without publishing integration events, so external consumers cannot react to lifecycle changes. The service already uses a .NET stack where RabbitMQ and MassTransit are approved messaging technologies, making them the preferred transport and abstraction for this change. The design must preserve existing REST API behavior and data persistence semantics while adding reliable, non-breaking event publication.

## Goals / Non-Goals

**Goals:**
- Publish `GameCreatedEvent` after successful game creation persistence.
- Publish `GameStateUpdatedEvent` after successful game update persistence.
- Configure MassTransit with RabbitMQ in the application composition root.
- Keep public API contracts backward compatible and avoid changing request/response payloads.
- Add automated tests validating publish behavior for success and failure paths.

**Non-Goals:**
- Implement or change event consumers.
- Introduce cross-service orchestration, sagas, or retries beyond MassTransit defaults.
- Redesign game domain models or gameplay rules.
- Replace or remove existing persistence mechanisms.

## Decisions

- Use MassTransit `IPublishEndpoint` from `GameStateService` to publish events.
  - Rationale: aligns with existing stack standards and keeps service logic simple.
  - Alternative considered: direct RabbitMQ client publishing; rejected due to lower consistency with platform conventions and higher plumbing effort.
- Publish only after successful data commit for create and update operations.
  - Rationale: prevents emitting events for failed writes and maintains producer consistency.
  - Alternative considered: publish before persistence; rejected because it can produce false-positive integration signals.
- Define explicit integration contracts (`GameCreatedEvent`, `GameStateUpdatedEvent`) in a shared contracts namespace/project used by the API.
  - Rationale: clear versionable event schema and stable consumer integration surface.
  - Alternative considered: anonymous/dynamic message objects; rejected due to weak contract governance.
- Include core event fields: game identifier, state payload summary, timestamps, and correlation metadata if available.
  - Rationale: gives consumers enough context for projections and tracing.
  - Alternative considered: minimal ID-only events; rejected because consumers would require extra read traffic.
- Gate publisher wiring with configuration so event publishing can be disabled operationally.
  - Rationale: supports safe rollout and rollback without code rollback in emergency scenarios.
  - Alternative considered: always-on publish; rejected due to reduced operational control.

## Risks / Trade-offs

- [Broker connectivity issues can increase request latency or failures] -> Mitigation: use MassTransit resilience defaults, timeouts, health checks, and monitor broker metrics.
- [At-least-once delivery can create duplicate processing downstream] -> Mitigation: document idempotency expectation for consumers and include stable event identifiers.
- [Event payload drift over time can break consumers] -> Mitigation: keep contracts additive and version events when introducing breaking changes.
- [Synchronous publish in request flow adds overhead] -> Mitigation: benchmark create/update endpoints and tune connection/channel settings.

## Migration Plan

1. Add event contracts and MassTransit + RabbitMQ configuration with environment-based settings.
2. Inject publishing abstraction (`IPublishEndpoint`) into `GameStateService`.
3. Emit `GameCreatedEvent` and `GameStateUpdatedEvent` after successful persistence operations.
4. Add unit/integration tests for positive and negative paths.
5. Deploy to non-production with publishing enabled and validate broker traffic.
6. Roll out to production with monitoring and alert thresholds.
7. Rollback strategy: disable publishing via configuration flag and redeploy (or flip environment setting), then investigate.

## Open Questions

- Should contract classes live in the existing API project or a dedicated shared contracts project?
- What exact game-state payload shape is required by first consumers to avoid under/over-sharing?
- Do we need explicit custom exchange/queue naming conventions beyond MassTransit defaults in this environment?

## Contract Versioning Expectations

- Event contracts are published under `GameStateService.Contracts.Events`.
- `GameCreatedEvent` and `GameStateUpdatedEvent` include a `SchemaVersion` field and start at `1.0`.
- Future changes should remain additive within the same major version.
- Breaking field changes require a new major schema version and dual-publish migration strategy.

## Operational Rollout and Rollback

- Rollout: enable `Messaging:EnableEventPublishing=true` in target environment and provide valid `Messaging:RabbitMq:*` values.
- Health checks: monitor default health endpoint and broker connectivity/producer errors after deployment.
- Rollback path: set `Messaging:EnableEventPublishing=false` to stop publishing while preserving existing REST and in-memory gameplay behavior.
- Incident response: keep feature toggle off until broker configuration or downstream consumer issues are resolved.
