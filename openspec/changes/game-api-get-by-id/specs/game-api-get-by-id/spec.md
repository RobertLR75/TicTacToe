## ADDED Requirements

### Requirement: GameService exposes get-by-id game state route
GameService SHALL expose `GET /api/games/{gameId}` as the public route for retrieving a single game's current state. The endpoint SHALL delegate read orchestration to a request handler or equivalent feature-local abstraction and SHALL return the shared game-state response contract.

#### Scenario: Existing game returns current state
- **WHEN** a client calls `GET /api/games/{gameId}` for an existing game
- **THEN** GameService SHALL return `200 OK`
- **AND** the response body SHALL include `gameId`, `currentPlayer`, `winner`, `isDraw`, `isOver`, and `board`
- **AND** the `board` collection SHALL contain the current persisted marks for that game state

#### Scenario: Missing game returns not found
- **WHEN** a client calls `GET /api/games/{gameId}` for a game that does not exist
- **THEN** GameService SHALL return `404 Not Found`

### Requirement: GameService get-by-id uses GameStateService as state source
GameService SHALL obtain live game-state data for `GET /api/games/{gameId}` from the service that owns current board state rather than reconstructing the board from GameService-only persistence.

#### Scenario: Read flow uses state owner response
- **WHEN** GameService handles `GET /api/games/{gameId}`
- **THEN** it SHALL request current state from the game-state owning dependency
- **AND** it SHALL map that result to the shared `GetGameResponse` contract without changing public field meanings

#### Scenario: State dependency unavailable
- **WHEN** GameService cannot retrieve current state because the game-state dependency is unavailable
- **THEN** GameService SHALL return a transient server error response
- **AND** the route SHALL NOT return a fabricated or partial game state
