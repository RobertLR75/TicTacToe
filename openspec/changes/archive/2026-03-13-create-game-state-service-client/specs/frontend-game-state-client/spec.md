## ADDED Requirements

### Requirement: Frontend uses a dedicated game-state client for game-state reads
The frontend SHALL call GameStateService game-state read endpoints through a dedicated typed client named `GameStateServiceClient` instead of using `GameApiClient`.

#### Scenario: Game page loads initial state through dedicated client
- **GIVEN** the user has created or selected a game to play
- **WHEN** the frontend requests the current state for that game
- **THEN** the request is sent through `GameStateServiceClient`
- **AND** the client targets the existing `GET /api/game-states/{gameId}` endpoint

### Requirement: Frontend uses a dedicated game-state client for move submission
The frontend SHALL submit game moves to GameStateService through `GameStateServiceClient` instead of using `GameApiClient`.

#### Scenario: User makes a move during gameplay
- **GIVEN** the game page has an active game and the selected cell is playable
- **WHEN** the user clicks a board cell to submit a move
- **THEN** the frontend sends the move through `GameStateServiceClient`
- **AND** the client targets the existing `POST /api/game-states/{gameId}/moves` endpoint

### Requirement: GameService client remains scoped to game catalog actions
The frontend SHALL keep `GameApiClient` responsible for GameService-backed create/list operations and SHALL NOT require it for game-state reads or move submission.

#### Scenario: Home page continues to create and list games through GameService client
- **GIVEN** the home page needs to list existing games or create a new game
- **WHEN** the page performs those actions
- **THEN** the frontend uses `GameApiClient` for create/list operations
- **AND** no game-state read or move endpoint is invoked through `GameApiClient`
