## MODIFIED Requirements

### Requirement: Game page displays interactive 3x3 board

The game page at route `/game` SHALL render a game list experience and a 3x3 grid of clickable cells using MudBlazor components. The page SHALL use `@rendermode InteractiveServer`.

#### Scenario: Game list is loaded when page opens

- **WHEN** a user navigates to `/game`
- **THEN** the page SHALL call `GET /api/games` to fetch created games
- **AND** display the returned games in a GameList view
- **AND** SHALL NOT automatically create a new game on initial page load

#### Scenario: Empty board displayed after creating a new game

- **WHEN** the user clicks the "New Game" button
- **THEN** the page SHALL call `POST /api/games` with `playerId` and `playerName`
- **AND** receive `202 Accepted` with `{ gameId }`
- **AND** join the SignalR game group using the returned `gameId`
- **AND** wait for the `GameCreated` notification to receive full game state
- **AND** display a 3x3 grid of 9 empty, clickable buttons
- **AND** show the current player as `X`

### Requirement: User can start a new game

The page SHALL provide a "New Game" button that creates a game with creator identity and refreshes the list of created games.

#### Scenario: New game button refreshes list after successful create

- **WHEN** the user clicks the "New Game" button
- **THEN** the system SHALL call `POST /api/games` with `playerId` and `playerName`
- **AND** the system SHALL call `GET /api/games` after the create request succeeds
- **AND** the refreshed GameList SHALL include the newly created game

#### Scenario: New game during active game

- **WHEN** the user clicks "New Game" while a game is in progress
- **THEN** the current game SHALL be abandoned (no cleanup API call needed)
- **AND** a new game SHALL be created
