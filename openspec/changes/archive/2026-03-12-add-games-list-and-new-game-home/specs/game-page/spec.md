## MODIFIED Requirements

### Requirement: User can start a new game
The game page SHALL provide a "New Game" button that creates a game with creator identity and refreshes the list of created games. The system SHALL also support the same create-and-refresh capability from the home page entry experience.

#### Scenario: New game button refreshes list after successful create
- **WHEN** the user clicks the "New Game" button
- **THEN** the system SHALL call `POST /api/games` with `playerId` and `playerName`
- **AND** the system SHALL call `GET /api/games` after the create request succeeds
- **AND** the refreshed GameList SHALL include the newly created game

#### Scenario: New game during active game
- **WHEN** the user clicks "New Game" while a game is in progress
- **THEN** the current game SHALL be abandoned (no cleanup API call needed)
- **AND** a new game SHALL be created

#### Scenario: Home page supports equivalent create-and-refresh behavior
- **WHEN** the user clicks the "New Game" action on the home page
- **THEN** the system SHALL call `POST /api/games` with `playerId` and `playerName`
- **AND** the system SHALL call `GET /api/games` after the create request succeeds
- **AND** the home page game list SHALL include the newly created game
