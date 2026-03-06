## Context

`POST /api/games` currently creates gameplay state but does not require explicit creator identity in the request contract, and the creation flow does not persist canonical game/player records in PostgreSQL. The requested change introduces request-level player identity and durable persistence using the existing shared infrastructure for Entity Framework and FluentMigrator.

Constraints and stakeholders:
- Preserve existing create-game response semantics (`202` with `gameId`) and SignalR notification behavior.
- Use `SharedLibrary.Postgree.EntityFramework` for persistence integration and `SharedLibrary.FluentMigration` for schema evolution.
- Minimize compatibility risk for existing API consumers affected by the new required request fields.
- Coordinate backend, client, and operations teams for rollout.

## Goals / Non-Goals

**Goals:**
- Require `playerId` (GUID) and `playerName` (string) on the create-game request contract.
- Persist `GameModel` and `PlayerModel` records when a game is created.
- Introduce migration(s) for game/player schema using FluentMigrator with clear rollback behavior.
- Maintain current response and event-notification behavior unless explicitly required otherwise.

**Non-Goals:**
- Changing game rules, turn logic, or board representation.
- Redesigning notification contracts or introducing new event buses.
- Implementing broader player profile management beyond create-game needs.

## Decisions

1. Treat player identity as required create-game input.
   - Decision: Extend `CreateGameRequest` with required `PlayerId` and `PlayerName`.
   - Rationale: Ensures every game creation has an explicit creator identity for persistence and traceability.
   - Alternative: Generate server-side identity placeholders.
   - Why not chosen: Produces weak identity guarantees and complicates downstream correlation.

2. Persist through shared EF integration package.
   - Decision: Map `GameModel` and `PlayerModel` using `SharedLibrary.Postgree.EntityFramework` patterns and save during create-game flow.
   - Rationale: Reuses established data access conventions and reduces one-off infrastructure code.
   - Alternative: Use Dapper-only writes for this endpoint.
   - Why not chosen: Inconsistent with requested approach and higher mapping duplication risk.

3. Manage schema with shared FluentMigrator tooling.
   - Decision: Add migration(s) creating required game/player tables and relationships with rollback path.
   - Rationale: Matches platform standard for repeatable, auditable schema changes.
   - Alternative: Manual SQL scripts.
   - Why not chosen: Harder to track, version, and roll back reliably across environments.

4. Keep external response shape stable while changing request contract.
   - Decision: Maintain `202` create response payload shape and existing `GameCreated` notification payload.
   - Rationale: Limits blast radius and keeps this change focused on identity capture + persistence.
   - Alternative: Expand response with player/game persistence metadata.
   - Why not chosen: Introduces additional client-facing contract churn without immediate product value.

## Risks / Trade-offs

- [Risk] Backward compatibility break for clients not sending `playerId`/`playerName` -> Mitigation: Communicate API version expectation, update clients in lockstep, and add contract tests.
- [Risk] Migration/runtime mismatch across environments -> Mitigation: Validate migrations in CI and pre-production using the same migration runner.
- [Risk] Additional DB write path increases create latency -> Mitigation: Keep inserts minimal, index key columns, and monitor create endpoint latency post-release.
- [Risk] Partial writes (game without player link) under failure -> Mitigation: Persist related records in a transaction boundary.

## Migration Plan

1. Add schema migration(s) for `PlayerModel` and `GameModel` (including relationships/constraints).
2. Deploy migration package to target environments and verify schema state.
3. Deploy GameService changes requiring new request fields and writing persistence records.
4. Roll out updated clients that provide `playerId` and `playerName`.
5. Monitor API validation failures, DB errors, and create latency.

Rollback strategy:
- Revert GameService deployment to previous version.
- Apply down migration (or compensating migration) to remove newly introduced schema objects if full rollback is required.

## Open Questions

- Should `playerName` uniqueness be enforced globally, per game, or not enforced at the database level?
- Do we need soft-delete/audit columns on `GameModel` and `PlayerModel` in this change, or follow-up change?
- Is the API contract update versioned explicitly, or coordinated as a synchronized breaking change with all consumers?
