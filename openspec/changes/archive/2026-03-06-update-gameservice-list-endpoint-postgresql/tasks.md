## 1. Data Access Migration

- [x] 1.1 Identify current `GET /api/games` list flow and remove `GameRepository` usage from the endpoint handler/service path.
- [x] 1.2 Implement a game-list specification object that captures existing filter, paging, and ordering inputs.
- [x] 1.3 Wire GameService list retrieval to `SharedLibrary.PostgreSQL` using `SearchAsync` with the new specification.

## 2. Repository Removal and Wiring

- [x] 2.1 Remove remaining `GameRepository` references from dependency injection and runtime code paths.
- [x] 2.2 Delete `GameRepository` and related obsolete abstractions/files once compile-time references are eliminated.
- [x] 2.3 Ensure endpoint response DTO/contract remains unchanged for backward compatibility.

## 3. Verification

- [x] 3.1 Add or update tests covering list filtering, paging, and ordering parity with pre-migration behavior.
- [x] 3.2 Run solution build and relevant unit/integration tests to confirm `SearchAsync`-based list retrieval passes.
- [x] 3.3 Validate query performance characteristics (index usage and representative list query latency) before merge.
