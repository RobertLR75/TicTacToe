## 1. API Contract Update

- [x] 1.1 Update `CreateGameRequest` to include required `PlayerId` (guid) and `PlayerName` (string).
- [x] 1.2 Update endpoint validation/binding to reject create-game requests missing `playerId` or `playerName`.
- [x] 1.3 Add or update API contract tests for valid and invalid create-game request payloads.

## 2. Persistence Model and Data Access

- [x] 2.1 Implement or update `PlayerModel` and `GameModel` entity mappings using `SharedLibrary.Postgree.EntityFramework` conventions.
- [x] 2.2 Wire GameService create flow to persist player and game data in PostgreSQL.
- [x] 2.3 Ensure create persistence runs in a transactional unit to prevent partial writes.

## 3. Database Migration

- [x] 3.1 Create FluentMigration migration(s) via `SharedLibrary.FluentMigration` for `PlayerModel` and `GameModel` schema objects.
- [x] 3.2 Implement migration rollback path (down migration or compensating migration) for introduced schema changes.
- [x] 3.3 Validate migration apply/rollback locally or in integration environment.

## 4. Compatibility and Verification

- [x] 4.1 Confirm `POST /api/games` success response remains `202` with `{ "gameId": "<id>" }` only.
- [x] 4.2 Confirm `GameCreated` notification payload shape remains unchanged.
- [x] 4.3 Run unit/integration tests covering request validation, persistence writes, and migration behavior.
