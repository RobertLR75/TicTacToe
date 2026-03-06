## Why

The GameService `List` endpoint currently relies on a repository path that does not use the shared PostgreSQL access conventions used elsewhere. Moving this endpoint to `SharedLibrary.PostgreSQL` with specification-based querying aligns data access patterns, reduces duplication, and makes filtering behavior consistent and maintainable.

## What Changes

- Update the GameService `List` endpoint to fetch games from PostgreSQL through `SharedLibrary.PostgreSQL`.
- Replace repository-driven list retrieval with specification-driven querying and `SearchAsync`.
- Remove `GameRepository` usage from the list flow and delete `GameRepository` once no longer referenced.
- Preserve existing endpoint contract and pagination/filter semantics for backward compatibility.

## Capabilities

### New Capabilities
- `list-games`: Define and implement specification-based game listing backed by PostgreSQL and `SearchAsync`.

### Modified Capabilities
- None.

## Impact

- Affected code: GameService list endpoint handler(s), data access wiring, and repository abstractions.
- API impact: No intentional breaking changes; endpoint shape and response contract remain stable.
- Dependencies: Introduces/standardizes dependency on `SharedLibrary.PostgreSQL` search/specification primitives in this flow.
- Affected teams: Backend API team (implementation), QA/integration testing (validation), frontend consumers (contract verification only).
- Performance: Query execution should improve or remain neutral through server-side filtering via specification and indexed PostgreSQL access; validate query plans and paging performance in integration tests.
- Rollback plan: Revert to prior list retrieval implementation and restore `GameRepository` path if regressions are detected, then redeploy with previous stable build.
