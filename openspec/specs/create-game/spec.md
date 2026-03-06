# create-game Specification

## Purpose
Defines the behavior of the `POST /api/games` endpoint for creating new games.

## Requirements
### Requirement: Full game state returned on creation

`POST /api/games` SHALL require a request body containing `playerId` (GUID) and `playerName` (string). On valid requests, the endpoint SHALL return `202 Accepted` with a minimal response containing only `gameId`. Full game state SHALL be delivered via the `GameCreated` SignalR notification to all connected clients. The endpoint SHALL delegate create-game orchestration to a request handler implementing the internal `IRequest<TResponse>` pattern.

#### Scenario: New game returns 202 with gameId only

- **WHEN** a client sends a valid `POST /api/games` request
- **THEN** the response status code SHALL be `202 Accepted`
- **AND** the response body SHALL contain only `{ "gameId": "<id>" }`
- **AND** the response body SHALL NOT contain `currentPlayer`, `winner`, `isDraw`, `isOver`, or `board`

#### Scenario: GameCreated notification sent after creation

- **WHEN** a client sends a valid `POST /api/games` request
- **THEN** the server SHALL call `NotificationService.NotifyGameCreated` with the new game state
- **AND** the `GameCreated` SignalR notification SHALL be sent to all connected clients
- **AND** the notification payload SHALL include `gameId`, `currentPlayer`, `winner`, `isDraw`, `isOver`, and `board`

#### Scenario: GameCreated notification shape matches GameUpdated shape

- **WHEN** a `GameCreated` notification is sent
- **THEN** its payload SHALL contain the same fields as `GameUpdateNotification`: `gameId`, `currentPlayer` (int), `winner` (int), `isDraw` (bool), `isOver` (bool), `board` (list of `{ row, col, mark }`)

#### Scenario: Request includes creator identity fields

- **WHEN** a client calls `POST /api/games`
- **THEN** the request body SHALL include `playerId` as a GUID value
- **AND** the request body SHALL include `playerName` as a non-empty string

#### Scenario: Missing creator identity is rejected

- **WHEN** a client sends `POST /api/games` without `playerId` or `playerName`
- **THEN** the endpoint SHALL reject the request with a client validation error response

#### Scenario: Create-game endpoint delegates to request handler

- **WHEN** `POST /api/games` is executed
- **THEN** create-game orchestration SHALL run in a `HandleAsync` request handler implementation
- **AND** endpoint logic SHALL remain limited to HTTP transport concerns
