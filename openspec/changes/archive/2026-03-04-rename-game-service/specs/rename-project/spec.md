## RENAMED Requirements

### Requirement: Project and namespace identity

The `GameService` project, namespace, and all associated references SHALL be renamed to `GameStateService`.

All existing API contracts, endpoint paths, request/response schemas, and runtime behavior SHALL remain unchanged.

#### Scenario: Namespace consistency after rename
- **WHEN** the project is renamed from `GameService` to `GameStateService`
- **THEN** all `.cs` files SHALL use `GameStateService` as the root namespace
- **AND** the project file SHALL be named `GameStateService.csproj`
- **AND** the project directory SHALL be `src/Backend/GameStateService/`

#### Scenario: Solution and orchestration references updated
- **WHEN** the project is renamed
- **THEN** `TicTacToe.sln` SHALL reference `GameStateService.csproj` at the new path
- **AND** the Aspire AppHost SHALL reference the renamed project

#### Scenario: API behavior unchanged
- **WHEN** the project is renamed
- **THEN** `POST /api/games` SHALL continue to create a new game and return full game state
- **AND** `GET /api/games/{id}` SHALL continue to return the game state for a given ID
- **AND** `POST /api/games/{id}/moves` SHALL continue to accept and process moves
