## ADDED Requirements

### Requirement: Typed HttpClient for GameAPI communication

The frontend SHALL use the `SharedLibrary.HttpClient` NuGet package to build a typed `GameApiClient` service that communicates with the GameService backend.

#### Scenario: GameApiClient is registered in DI

- **WHEN** the application starts
- **THEN** `GameApiClient` SHALL be registered in DI via `AddHttpClient<GameApiClient>` with a configured base address pointing to the GameService

#### Scenario: Create a new game

- **WHEN** `GameApiClient.CreateGameAsync()` is called
- **THEN** it SHALL send `POST /api/games` to the GameService
- **AND** return a `GameResponse` containing `gameId`, `currentPlayer`, `winner`, `isDraw`, `isOver`, and `board`

#### Scenario: Get game state

- **WHEN** `GameApiClient.GetGameAsync(gameId)` is called
- **THEN** it SHALL send `GET /api/games/{gameId}` to the GameService
- **AND** return a `GameResponse` with the current game state

#### Scenario: Make a move

- **WHEN** `GameApiClient.MakeMoveAsync(gameId, row, col)` is called
- **THEN** it SHALL send `POST /api/games/{gameId}/moves` with `{ gameId, row, col }` as JSON body
- **AND** return a `GameResponse` with the updated game state
