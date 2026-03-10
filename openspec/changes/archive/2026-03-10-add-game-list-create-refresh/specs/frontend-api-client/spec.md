## MODIFIED Requirements

### Requirement: Typed HttpClient for GameAPI communication

The frontend SHALL use a typed `GameApiClient` service that communicates with the GameStateService backend. Return types SHALL reflect the `202 Accepted` response pattern for create and move operations while preserving typed response models for read operations.

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

#### Scenario: Get game state

- **WHEN** `GameApiClient.GetGameAsync(gameId)` is called
- **THEN** it SHALL send `GET /api/games/{gameId}` to the GameService
- **AND** return a `GameResponse` with the current game state

#### Scenario: Make a move

- **WHEN** `GameApiClient.MakeMoveAsync(gameId, row, col)` is called
- **THEN** it SHALL send `POST /api/games/{gameId}/moves` with `{ gameId, row, col }` as JSON body
- **AND** ensure the response is `202 Accepted`
- **AND** return `void` (no response body to parse)
