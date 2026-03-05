## MODIFIED Requirements

### Requirement: Game page displays interactive 3x3 board

The game page at route `/game` SHALL render a 3x3 grid of clickable cells using MudBlazor components. Each cell SHALL display the current mark (`X`, `O`, or empty). The page SHALL use `@rendermode InteractiveServer`.

#### Scenario: Empty board displayed on new game

- **WHEN** a user navigates to `/game`
- **THEN** the page SHALL call `POST /api/games` to create a new game
- **AND** receive `202 Accepted` with `{ gameId }`
- **AND** join the SignalR game group using the returned `gameId`
- **AND** wait for the `GameCreated` notification to receive full game state
- **AND** display a 3x3 grid of 9 empty, clickable buttons
- **AND** show the current player as `X`

#### Scenario: Cell displays placed mark

- **WHEN** a move has been made on cell (row=0, col=1) by player `X`
- **THEN** the cell at row 0, column 1 SHALL display `X`
- **AND** the cell SHALL be visually distinct from empty cells

### Requirement: User can make a move by clicking a cell

The system SHALL submit a move to the backend when a user clicks an empty cell. The board SHALL update when the `GameUpdated` SignalR notification is received, not from the HTTP response.

#### Scenario: Successful move submission

- **WHEN** it is player `X`'s turn
- **AND** the user clicks an empty cell at row 1, col 1
- **THEN** the system SHALL call `POST /api/games/{gameId}/moves` with `{ row: 1, col: 1 }`
- **AND** receive `202 Accepted` with no response body
- **AND** show a loading indicator until the `GameUpdated` notification arrives
- **AND** update the board with the state from the `GameUpdated` notification
- **AND** switch the displayed current player to `O`

#### Scenario: Clicking an occupied cell is prevented

- **WHEN** a cell already contains a mark (`X` or `O`)
- **THEN** the cell button SHALL be disabled
- **AND** no API call SHALL be made

#### Scenario: Clicking during game over is prevented

- **WHEN** the game is over (win or draw)
- **THEN** all cell buttons SHALL be disabled
- **AND** no API call SHALL be made

### Requirement: Game page receives state from GameCreated notification

The game page SHALL subscribe to `GameCreated` notifications and use them to populate initial game state after creating a new game.

#### Scenario: GameCreated notification populates board

- **WHEN** the page has called `POST /api/games` and joined the SignalR group
- **AND** a `GameCreated` notification is received with matching `gameId`
- **THEN** the page SHALL call `UpdateState` with the notification's game state
- **AND** render the board and status from the notification data

#### Scenario: GameCreated notification for different game is ignored

- **WHEN** a `GameCreated` notification is received
- **AND** its `gameId` does not match the current game
- **THEN** the page SHALL ignore the notification

### Requirement: Game status is displayed to the user

The page SHALL display the current game status: whose turn it is, the winner, or if the game is a draw.

#### Scenario: Current player turn displayed

- **WHEN** the game is in progress
- **THEN** the page SHALL display text indicating whose turn it is (e.g., "Player X's turn")

#### Scenario: Winner displayed

- **WHEN** a player wins the game (notification returns `isOver: true` and `winner` is `X` or `O`)
- **THEN** the page SHALL display the winner (e.g., "Player X wins!")
- **AND** all cell buttons SHALL be disabled

#### Scenario: Draw displayed

- **WHEN** the game ends in a draw (notification returns `isOver: true` and `isDraw: true`)
- **THEN** the page SHALL display "It's a draw!"
- **AND** all cell buttons SHALL be disabled

### Requirement: User can start a new game

The page SHALL provide a button to start a new game at any time.

#### Scenario: New game button resets the board

- **WHEN** the user clicks the "New Game" button
- **THEN** the system SHALL leave the previous SignalR game group
- **AND** call `POST /api/games` to create a new game
- **AND** join the new game's SignalR group using the returned `gameId`
- **AND** wait for the `GameCreated` notification to receive full state
- **AND** replace the board with the new game state
- **AND** reset the displayed status to "Player X's turn"

#### Scenario: New game during active game

- **WHEN** the user clicks "New Game" while a game is in progress
- **THEN** the current game SHALL be abandoned (no cleanup API call needed)
- **AND** a new game SHALL be created

### Requirement: Navigation includes Game page link

The sidebar navigation SHALL include a link to the Game page and SHALL NOT include links to Counter or Weather pages.

#### Scenario: Nav menu shows Game link

- **WHEN** the application loads
- **THEN** the navigation menu SHALL display a "Game" link pointing to `/game`
- **AND** SHALL NOT display "Counter" or "Weather" links

### Requirement: API errors are communicated to the user

The page SHALL display an error message when API calls fail.

#### Scenario: API failure on move

- **WHEN** the user makes a move and the API returns an error
- **THEN** the page SHALL display an error notification (e.g., MudBlazor Snackbar)
- **AND** the board state SHALL remain unchanged
