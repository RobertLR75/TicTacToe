# Spec: create-game

## ADDED Requirements

### Requirement: Full game state returned on creation

`POST /api/games` SHALL return all game state fields so that clients can initialize without a separate GET request.

#### Scenario: New game response includes game-over fields

- **WHEN** a client sends `POST /api/games`
- **THEN** the response body includes `winner`, `isDraw`, and `isOver`
- **AND** `winner` equals `None` (0)
- **AND** `isDraw` equals `false`
- **AND** `isOver` equals `false`

#### Scenario: Create-game response shape matches get-game response shape

- **WHEN** a client compares the fields of `POST /api/games` and `GET /api/games/{id}`
- **THEN** both responses contain the same top-level fields: `gameId`, `currentPlayer`, `winner`, `isDraw`, `isOver`, `board`
