## MODIFIED Requirements

### Requirement: Project and namespace identity

The `GameStateService` project, namespace, and all associated references SHALL use `GameStateService` as the canonical name.

The `GameService` project (formerly `GameApi`), namespace, and all associated references SHALL use `GameService` as the canonical name.

All existing API contracts, endpoint paths, request/response schemas, and runtime behavior SHALL remain unchanged regardless of project naming.

#### Scenario: Namespace consistency for GameStateService
- **WHEN** the project is built
- **THEN** all `.cs` files in the GameStateService project SHALL use `GameStateService` as the root namespace
- **AND** the project file SHALL be named `GameStateService.csproj`
- **AND** the project directory SHALL be `src/Backend/GameStateService/`

#### Scenario: Namespace consistency for GameService
- **WHEN** the project is built
- **THEN** all `.cs` files in the GameService project SHALL use `GameService` as the root namespace
- **AND** the project file SHALL be named `GameService.csproj`
- **AND** the project directory SHALL be `src/Backend/GameService/`

#### Scenario: Solution and orchestration references
- **WHEN** the solution is loaded
- **THEN** `TicTacToe.sln` SHALL reference `GameStateService.csproj` at the correct path
- **AND** `TicTacToe.sln` SHALL reference `GameService.csproj` at the correct path
- **AND** the Aspire AppHost SHALL reference both the `GameStateService` and `GameService` projects

#### Scenario: Aspire resource identity for GameService
- **WHEN** the Aspire AppHost is started
- **THEN** the GameService project SHALL be registered with resource name `"gameservice"`
- **AND** the HTTP endpoint SHALL be named `"gameservice-http"`

#### Scenario: API behavior unchanged for GameStateService
- **WHEN** the GameStateService is running
- **THEN** `POST /api/games` SHALL create a new game and return full game state
- **AND** `GET /api/games/{id}` SHALL return the game state for a given ID
- **AND** `POST /api/games/{id}/moves` SHALL accept and process moves

#### Scenario: API behavior unchanged for GameService
- **WHEN** the GameService is running
- **THEN** `POST /api/game-lobby` SHALL create a new game lobby
- **AND** `GET /api/game-lobby` SHALL list games with status Created
- **AND** `PUT /api/game-lobby/{Id}/activate` SHALL transition a game from Created to Active
- **AND** `PUT /api/game-lobby/{Id}/complete` SHALL transition a game from Active to Completed
