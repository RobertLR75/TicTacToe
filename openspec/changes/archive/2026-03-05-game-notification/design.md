## Context

`GameNotificationService` currently contains a placeholder notifications API that always returns an empty array and does not ingest game events. The platform stack already supports RabbitMQ + MassTransit and includes event publishers for game lifecycle updates, so this change wires the notification service into that event stream and adds persistence-backed reads while maintaining the existing API route shape.

## Goals / Non-Goals

**Goals:**
- Consume `GameCreated` and `GameStateUpdated` events in `GameNotificationService`.
- Persist notification records suitable for API retrieval.
- Replace stub notification retrieval with database-backed results including pagination/filtering.
- Preserve backward compatibility of endpoint route and response contract expectations.
- Support safe rollout and rollback through configuration toggles.

**Non-Goals:**
- Implement push delivery channels (email/SMS/websocket) in this change.
- Introduce user preference management or notification mute logic.
- Redesign game event contracts published by upstream services.

## Decisions

- Use MassTransit consumers in `GameNotificationService` for event ingestion.
  - Rationale: aligns with approved messaging patterns and minimizes custom transport code.
  - Alternative considered: direct RabbitMQ client consumer; rejected due to additional plumbing and lower consistency.
- Store notifications in a dedicated persistence model (table/entity) with timestamps and event correlation fields.
  - Rationale: supports deterministic API reads and future auditing.
  - Alternative considered: in-memory cache only; rejected because data is lost on restart and not queryable across instances.
- Keep `GET /api/notifications` endpoint path unchanged and evolve internals from stub to data-backed read.
  - Rationale: preserves existing client integration points and backward compatibility.
  - Alternative considered: new versioned endpoint only; rejected because current contract can evolve safely with additive fields.
- Add query defaults (page size limit and newest-first ordering) for bounded performance.
  - Rationale: avoids unbounded scans and protects service latency.
  - Alternative considered: return all records; rejected for scalability concerns.
- Add a feature toggle for event processing enablement.
  - Rationale: enables operational rollback without full deployment rollback.
  - Alternative considered: always-on processing; rejected for reduced incident response flexibility.

## Risks / Trade-offs

- [Consumer throughput spikes can increase DB write pressure] -> Mitigation: batch/optimize writes later, add indexes now, and enforce pagination on reads.
- [Duplicate event delivery can create duplicate notifications] -> Mitigation: include event identifiers and apply idempotency guard strategy in persistence.
- [Schema changes for notifications require careful migration] -> Mitigation: use FluentMigrator with backward-compatible additive migrations.
- [Toggle misconfiguration may silently disable processing] -> Mitigation: surface toggle state at startup logs and health diagnostics.

## Migration Plan

1. Add notification persistence model and migration scripts.
2. Add MassTransit consumer wiring and implement event-to-notification mapping.
3. Update `GET /api/notifications` endpoint to read from persistence with pagination/filter options.
4. Add tests for consumer processing, duplicate handling expectations, and endpoint compatibility.
5. Deploy to non-production with processing enabled and validate end-to-end flow.
6. Roll out to production with monitoring dashboards and alerting.
7. Rollback strategy: disable processing via feature flag and redeploy previous version if necessary.

## Open Questions

- Should notifications be keyed by player/user identity now, or left as game-scoped records in this phase?
- What retention window is required for stored notifications (days/weeks) before archival or cleanup?
- Should idempotency be strict (unique event id constraint) or best-effort in initial rollout?
