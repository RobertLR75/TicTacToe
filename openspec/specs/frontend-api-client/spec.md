# frontend-api-client Specification

## Purpose
Defines the typed HttpClient used by the frontend to communicate with the GameService backend for game creation and listing.

## Requirements

### Requirement: Typed HttpClient for GameService communication

The frontend SHALL use a typed `GameApiClient` service that communicates with the GameService backend for game creation and listing. `GameApiClient` SHALL NOT own game-state reads or move submission.

#### Scenario: GameApiClient is registered in DI

- **WHEN** the application starts
- **THEN** `GameApiClient` SHALL be registered in DI via `AddHttpClient<GameApiClient>` with a configured base address pointing to the GameService

#### Scenario: Create a new game

- **WHEN** `GameApiClient.CreateGameAsync(playerId, playerName)` is called
- **THEN** it SHALL send `POST /api/games` to the GameService with JSON body containing `playerId` and `playerName`
- **AND** return a `string` containing the `gameId` from the `202 Accepted` response
- **AND** SHALL NOT return full game state (no `GameResponse`)

#### Scenario: List created games

- **WHEN** `GameApiClient.ListGamesAsync()` is called
- **THEN** it SHALL send `GET /api/games` to the GameService
- **AND** return a typed list response containing created games

#### Scenario: GameApiClient excludes game-state reads and move submission

- **WHEN** the frontend needs to load a game state or submit a move during gameplay
- **THEN** those operations SHALL be handled by a dedicated GameStateService client
- **AND** `GameApiClient` SHALL remain scoped to create/list operations only
