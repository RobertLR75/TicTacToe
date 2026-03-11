# gamestate-request-handler-pattern Specification

## Purpose
Defines the internal request-handler architecture for GameStateService operations.

## Requirements
### Requirement: Game state operations SHALL use IRequest handler contracts
GameStateService SHALL define an internal request abstraction where each command or query is represented as an `IRequest<TResponse>` and is processed by a corresponding handler that exposes `HandleAsync`.

#### Scenario: Request contract exists for feature operations
- **WHEN** a new game-state command or query is introduced
- **THEN** the operation SHALL be represented by a request type implementing `IRequest<TResponse>`
- **AND** a corresponding handler type SHALL be defined for that request

### Requirement: Handlers SHALL centralize operation orchestration
GameStateService handlers SHALL own orchestration for create game, get game, and make move operations, including use of existing collaborators such as repositories, game logic services, and event publishers, and this orchestration SHALL be covered by unit tests for success and failure paths.

#### Scenario: Create game flow executes through a handler
- **GIVEN** a create game command is issued
- **WHEN** the create game operation is invoked
- **THEN** orchestration SHALL execute in a dedicated `HandleAsync` handler implementation
- **AND** the handler SHALL use existing persistence and event publication dependencies

#### Scenario: Make move flow executes through a handler
- **GIVEN** a make move command is issued
- **WHEN** the make move operation is invoked
- **THEN** orchestration SHALL execute in a dedicated `HandleAsync` handler implementation
- **AND** game rules and persistence collaborators SHALL be invoked from the handler

#### Scenario: Handler branches are validated by unit tests
- **GIVEN** request handlers with multiple outcome branches
- **WHEN** GameStateService unit tests are executed
- **THEN** each branch SHALL be asserted for returned status and dependency interactions

### Requirement: Endpoint contracts SHALL remain transport-only and backward compatible
FastEndpoints endpoint classes in GameStateService SHALL remain responsible for HTTP transport concerns and SHALL delegate operation execution to request handlers without changing public endpoint contracts, and endpoint parity tests SHALL enforce this behavior.

#### Scenario: Endpoint delegates without schema change
- **GIVEN** existing API clients
- **WHEN** clients call existing game-state endpoints after refactor
- **THEN** request and response payload structures SHALL remain backward compatible
- **AND** endpoint execution SHALL delegate to an internal request handler

#### Scenario: Endpoint contract parity tests protect handler delegation
- **GIVEN** endpoint parity unit tests for GameStateService
- **WHEN** constructor and base-type contracts are validated
- **THEN** each endpoint SHALL keep the expected request/response contract and handler dependency type

### Requirement: Request handlers SHALL be colocated with their FastEndpoints
Each GameStateService request handler class SHALL be placed in the same feature folder as its corresponding FastEndpoint class.

#### Scenario: Create-game handler file placement
- **WHEN** the create-game request handler is added
- **THEN** its class file SHALL be located in the same folder as the create-game FastEndpoint class

#### Scenario: Make-move handler file placement
- **WHEN** the make-move request handler is added
- **THEN** its class file SHALL be located in the same folder as the make-move FastEndpoint class
