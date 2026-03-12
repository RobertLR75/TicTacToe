# game-persistence Specification

## Purpose
Define persistence and migration requirements for saving game and player data during game creation.

## Requirements
### Requirement: Persist game and creating player on game creation

The system SHALL persist a `PlayerModel` and `GameModel` record in PostgreSQL when `POST /api/games` succeeds, using `SharedLibrary.Postgree.EntityFramework` data access integration. GameService startup SHALL resolve the PostgreSQL connection for this persistence path from supported runtime configuration sources, including the existing connection-string configuration path.

#### Scenario: Successful create persists player and game

- **WHEN** a valid `POST /api/games` request is processed successfully
- **THEN** a `PlayerModel` record SHALL be created or linked for the provided `playerId` and `playerName`
- **AND** a `GameModel` record SHALL be created for the new game
- **AND** the `GameModel` SHALL reference the creating player identity

#### Scenario: Persistence uses one transactional unit

- **WHEN** game creation persistence executes
- **THEN** player and game writes SHALL be committed atomically
- **AND** the system MUST NOT leave partial persisted state for a failed create operation

#### Scenario: Startup accepts supported persistence configuration sources

- **WHEN** GameService starts with PostgreSQL configuration provided through a supported runtime source
- **THEN** the persistence registration SHALL accept that configuration
- **AND** the service SHALL be ready to persist created games without requiring manual remapping to a single hard-coded key

### Requirement: Database schema managed with FluentMigration

The system SHALL define and apply migration(s) for `GameModel` and `PlayerModel` using `SharedLibrary.FluentMigration` conventions.

#### Scenario: Migration creates required schema objects

- **WHEN** the migration up path is executed
- **THEN** required tables for `GameModel` and `PlayerModel` SHALL be created in PostgreSQL
- **AND** required keys and relationships SHALL be applied

#### Scenario: Migration supports rollback

- **WHEN** rollback is required
- **THEN** the migration down path (or compensating migration) SHALL remove or safely reverse schema objects introduced by this change
