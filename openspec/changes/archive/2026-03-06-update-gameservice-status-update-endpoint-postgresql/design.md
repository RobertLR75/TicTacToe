## Context

Game status transitions are currently split across separate Complete and Activate endpoints in GameService. This duplicates validation and persistence paths, increases API surface area, and forces clients to encode endpoint-specific behavior instead of sending a single status intent. The service uses FastEndpoints and persists game data in PostgreSQL through existing repository patterns.

## Goals / Non-Goals

**Goals:**
- Provide one status update endpoint that accepts `Active` and `Completed` transitions for an existing game.
- Centralize transition validation and persistence in one handler path.
- Preserve backward compatibility during migration by deprecating old routes before removal.
- Ensure PostgreSQL updates are consistently keyed by game id and write status atomically.

**Non-Goals:**
- Redesign unrelated gameplay update endpoints (board move updates remain unchanged).
- Introduce new status values beyond `Active` and `Completed`.
- Change external messaging infrastructure or notification transport protocols.

## Decisions

1. Introduce a single endpoint (for example, `PUT /api/games/{id}/status`) with a request body containing the target status.
   - Rationale: One endpoint simplifies client integration and removes duplicated route logic.
   - Alternatives considered:
     - Keep both endpoints and share internal service logic only: reduces duplication in code but retains fragmented API contract.
     - Use generic game update endpoint for all fields: too broad for this focused status lifecycle change.

2. Restrict allowed status transitions in the endpoint contract and validation layer to `Active` and `Completed`.
   - Rationale: Explicit validation keeps behavior predictable and avoids invalid state writes.
   - Alternatives considered:
     - Accept free-form enum values and reject later in persistence: increases error ambiguity and weakens API contract.

3. Implement persistence with game id as the canonical key and status as the only mutated state in this endpoint path.
   - Rationale: Prevents accidental updates to unrelated fields and aligns with intended endpoint responsibility.
   - Alternatives considered:
     - Reuse broader update command object: risks unintended field coupling and future regressions.

4. Use a staged migration for backward compatibility.
   - Rationale: Existing clients may still call Complete/Activate routes and need transition time.
   - Alternatives considered:
     - Immediate endpoint removal: lower maintenance but higher release risk for consumers.

## Risks / Trade-offs

- [Clients continue using deprecated Complete/Activate endpoints] -> Mitigation: keep temporary compatibility routes that map to new handler and emit deprecation warnings.
- [Invalid status transition requests increase after endpoint consolidation] -> Mitigation: enforce strict request validation with clear error payloads and API examples.
- [Persistence regression in PostgreSQL update path] -> Mitigation: add integration tests covering id-based update behavior for both `Active` and `Completed`.
- [Dual-route support temporarily increases maintenance burden] -> Mitigation: set a deprecation timeline and remove legacy routes in a planned follow-up release.

## Migration Plan

1. Add the unified status update endpoint and internal handler/service path.
2. Keep Complete/Activate endpoints as compatibility wrappers that call the new status update logic.
3. Update API documentation and client examples to use the unified endpoint.
4. Add telemetry/deprecation logs for legacy route usage and monitor adoption.
5. Remove legacy routes after adoption threshold and release window criteria are met.
6. Rollback strategy: re-enable legacy endpoint handling as primary routes and redeploy previous stable package if critical client impact occurs.

## Open Questions

- Should legacy routes return deprecation headers only, or also include response body warnings for easier client visibility?
- Is there any existing client that depends on endpoint-specific authorization policy differences that must be preserved?
