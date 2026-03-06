## Context

GameService currently handles list retrieval through a repository implementation that does not follow the shared PostgreSQL query patterns used in other services. The requested change moves list retrieval to `SharedLibrary.PostgreSQL` and adopts specification-based querying via `SearchAsync` so filtering, paging, and query composition are centralized and consistent.

This endpoint is consumed by clients expecting stable response shape and behavior, so backward compatibility is required.

## Goals / Non-Goals

**Goals:**
- Route `List` endpoint data access through `SharedLibrary.PostgreSQL`.
- Implement list filtering and paging using a dedicated specification and `SearchAsync`.
- Remove `GameRepository` from the list flow and delete it if no longer referenced.
- Preserve existing API contract and maintain equivalent or improved query performance.

**Non-Goals:**
- Redesign the endpoint contract or introduce breaking API changes.
- Rework unrelated GameService endpoints.
- Introduce new storage engines or data access frameworks.

## Decisions

1. Use a specification object for game listing criteria.
   - Rationale: Encapsulates query logic in a testable unit and aligns with `SharedLibrary.PostgreSQL` conventions.
   - Alternative considered: Inline SQL in endpoint handler; rejected because it duplicates query logic and increases maintenance risk.

2. Use `SearchAsync` for list retrieval.
   - Rationale: Provides standardized query execution and mapping behavior expected by shared data access tooling.
   - Alternative considered: Keep custom repository methods; rejected because it prolongs divergence from shared patterns.

3. Remove `GameRepository` after migration.
   - Rationale: Eliminates dead abstraction and reduces confusion over supported data access path.
   - Alternative considered: Keep repository as a thin wrapper; rejected because it adds indirection without value.

4. Preserve response DTO and ordering/paging semantics unless existing behavior is undefined.
   - Rationale: Maintains compatibility for downstream clients.
   - Alternative considered: Normalize response and sorting defaults during migration; rejected to avoid accidental behavioral breaks.

## Risks / Trade-offs

- [Query behavior drift] Existing repository filters may not map 1:1 to new specification logic. -> Mitigation: Add/adjust integration tests for filter combinations, paging, and ordering.
- [Performance regression] New generated SQL could perform worse on large datasets. -> Mitigation: Validate query plan against representative data and verify index usage.
- [Hidden dependencies on repository] Other components may still reference `GameRepository`. -> Mitigation: Run full build/tests after removal and update DI registrations.

## Migration Plan

1. Implement list specification and wire endpoint to `SearchAsync`.
2. Update dependency injection and remove `GameRepository` references.
3. Run unit and integration tests for list endpoint behavior and performance-sensitive scenarios.
4. Deploy through normal pipeline.
5. Rollback strategy: redeploy previous stable version and restore repository-backed list path if production issues arise.

## Open Questions

- Which exact filters (status/player/date/search text) are currently expected by clients and must be preserved verbatim?
- Does `SearchAsync` already include default ordering, or should explicit ordering be specified in the list specification?
