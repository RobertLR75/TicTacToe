## Why

The game creation API currently lacks explicit player identity details in the request and does not persist created game/player records in PostgreSQL. We need to capture creator identity at creation time and store canonical game data to support reliable downstream workflows and future query/use cases.

## What Changes

- Update `POST /api/games` request contract to require `CreateGameRequest.PlayerId` (`guid`) and `CreateGameRequest.PlayerName` (`string`).
- Persist new game and creating player data in PostgreSQL via `SharedLibrary.Postgree.EntityFramework`.
- Add FluentMigrator migration(s) using `SharedLibrary.FluentMigration` to create database structures for `GameModel` and `PlayerModel`.
- Keep existing response/notification flow intact unless required by schema updates; this change focuses on request contract and persistence.
- Define rollout and rollback steps for schema and API contract changes.

## Capabilities

### New Capabilities
- `game-persistence`: Persists newly created games and players in PostgreSQL with migrations and EF mappings.

### Modified Capabilities
- `create-game`: The create-game endpoint request requirements change to include `playerId` and `playerName`.

## Impact

- Affected code: GameService create endpoint, request DTOs/validation, EF DbContext/entities/repositories, migration project.
- Affected APIs: `POST /api/games` request payload (potentially **BREAKING** for clients not sending the new fields).
- Dependencies/systems: `SharedLibrary.Postgree.EntityFramework`, `SharedLibrary.FluentMigration`, PostgreSQL schema management.
- Affected teams: GameService backend team, API consumers (frontend/integration clients), DevOps/DBA for migration rollout.
- Performance impact: Small write overhead on game creation due to DB insert operations; expected minimal impact with indexed keys and simple insert path.
- Rollback plan: Revert service deployment and run a down migration (or compensating migration) to remove newly introduced tables/columns if release must be rolled back.
