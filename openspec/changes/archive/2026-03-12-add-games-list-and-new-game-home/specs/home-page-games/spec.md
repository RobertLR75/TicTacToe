## ADDED Requirements

### Requirement: Home page lists created games
The home page SHALL load and display created games by calling `GameApiClient.ListGamesAsync()` when the page initializes.

#### Scenario: Home page loads existing games on initial render
- **GIVEN** the user can access the home page
- **WHEN** the home page is rendered
- **THEN** the frontend calls `GameApiClient.ListGamesAsync()` once
- **AND** the returned games are displayed in a list view on the home page

#### Scenario: Home page shows empty state when no games exist
- **GIVEN** `GameApiClient.ListGamesAsync()` returns no games
- **WHEN** the home page is rendered
- **THEN** the page displays an empty-state message indicating no games are available

### Requirement: Home page provides New Game action
The home page SHALL provide a "New Game" action that creates a game via `GameApiClient.CreateGameAsync(playerId, playerName)` using the current user identity context.

#### Scenario: New game is created from home page
- **GIVEN** a valid current user identity exists
- **WHEN** the user clicks the "New Game" button on the home page
- **THEN** the frontend calls `GameApiClient.CreateGameAsync(playerId, playerName)`
- **AND** the create request is treated as successful only when the API returns `202 Accepted` with a `gameId`

### Requirement: Home page refreshes game list after successful creation
After a successful home-page create action, the system SHALL refresh the game list by calling `GameApiClient.ListGamesAsync()` so the newly created game appears in the list.

#### Scenario: Newly created game appears after refresh
- **GIVEN** the user successfully creates a game from the home page
- **WHEN** create returns a valid `gameId`
- **THEN** the frontend calls `GameApiClient.ListGamesAsync()` again
- **AND** the home-page list includes the newly created game entry

### Requirement: Home page communicates API outcomes to users
The home page SHALL present user-friendly notifications for list and create outcomes.

#### Scenario: List API failure shows error notification
- **GIVEN** the home page is loading games
- **WHEN** `GameApiClient.ListGamesAsync()` fails
- **THEN** the page shows an error notification
- **AND** the page remains interactive so the user can retry

#### Scenario: Create API failure shows error notification
- **GIVEN** the user clicks "New Game"
- **WHEN** `GameApiClient.CreateGameAsync(...)` fails
- **THEN** the page shows an error notification
- **AND** the existing game list remains unchanged
