## MODIFIED Requirements

### Requirement: Game page displays interactive 3x3 board

The game page at route `/game` SHALL render a 3x3 grid of clickable cells using MudBlazor components. Each cell SHALL display the current mark (`X`, `O`, or empty). The page SHALL use `@rendermode InteractiveServer`.

#### Scenario: Empty board displayed on new game

- **GIVEN** a user opens the game page
- **WHEN** the user navigates to `/game`
- **THEN** the page SHALL call `POST /api/games` to create a new game
- **AND** receive `202 Accepted` with `{ gameId }`
- **AND** join the SignalR game group for that `gameId` using the `GameNotificationService` SignalR connection
- **AND** wait for the `GameCreatedNotification` event to receive full game state
- **AND** display a 3x3 grid of 9 empty, clickable buttons
- **AND** show the current player as `X`

#### Scenario: Cell displays placed mark

- **GIVEN** a game state has been received
- **WHEN** a move has been made on cell (row=0, col=1) by player `X`
- **THEN** the cell at row 0, column 1 SHALL display `X`
- **AND** the cell SHALL be visually distinct from empty cells

### Requirement: User can make a move by clicking a cell

The system SHALL submit a move to the backend when a user clicks an empty cell. The board SHALL update when the `GameStateUpdatedNotification` SignalR event is received from `GameNotificationService`, not from the HTTP response.

#### Scenario: Successful move submission

- **GIVEN** it is player `X`'s turn
- **WHEN** the user clicks an empty cell at row 1, col 1
- **THEN** the system SHALL call `POST /api/games/{gameId}/moves` with `{ row: 1, col: 1 }`
- **AND** receive `202 Accepted` with no response body
- **AND** show a loading indicator until the `GameStateUpdatedNotification` event arrives
- **AND** update the board with the state from the `GameStateUpdatedNotification` event
- **AND** switch the displayed current player to `O`

#### Scenario: Clicking an occupied cell is prevented

- **GIVEN** a cell already contains a mark (`X` or `O`)
- **WHEN** the user attempts to click the occupied cell
- **THEN** the cell button SHALL be disabled
- **AND** no API call SHALL be made

#### Scenario: Clicking during game over is prevented

- **GIVEN** the game is over (win or draw)
- **WHEN** the user attempts to click any board cell
- **THEN** all cell buttons SHALL be disabled
- **AND** no API call SHALL be made

### Requirement: Game page receives state from GameCreated notification

The game page SHALL subscribe through `GameNotificationService` SignalR connection to `GameCreatedNotification` events and use them to populate initial game state after creating a new game.

#### Scenario: GameCreated notification populates board

- **GIVEN** the page has called `POST /api/games` and joined the SignalR group
- **WHEN** a `GameCreatedNotification` event is received with matching `gameId`
- **THEN** the page SHALL call `UpdateState` with the notification's game state
- **AND** render the board and status from the notification data

#### Scenario: GameCreated notification for different game is ignored

- **GIVEN** the page is currently tracking a specific `gameId`
- **WHEN** a `GameCreatedNotification` event is received for a different `gameId`
- **THEN** the page SHALL ignore the notification
