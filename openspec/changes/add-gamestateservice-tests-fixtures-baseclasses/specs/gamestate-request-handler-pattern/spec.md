## MODIFIED Requirements

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
