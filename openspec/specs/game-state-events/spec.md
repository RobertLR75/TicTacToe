## Requirements

### Requirement: Publish game created integration event
The system SHALL publish a `GameCreatedEvent` to RabbitMQ through MassTransit when `GameStateService` successfully creates and persists a new game, and integration tests SHALL verify event publication behavior under containerized runtime dependencies.

#### Scenario: Game creation succeeds and event is published
- **GIVEN** a valid create game request and healthy messaging infrastructure
- **WHEN** `GameStateService` completes a new game creation request and persistence succeeds
- **THEN** the system publishes exactly one `GameCreatedEvent` containing the game identifier and initial state details

#### Scenario: Game creation fails and no event is published
- **GIVEN** a create game flow that fails before persistence commit
- **WHEN** the operation exits with failure
- **THEN** the system MUST NOT publish a `GameCreatedEvent`

#### Scenario: Integration tests validate game created publication path
- **GIVEN** Testcontainers-based dependencies for GameStateService integration tests
- **WHEN** the create game runtime path is executed in integration tests
- **THEN** the suite SHALL assert that a `GameCreatedEvent` is published only for successful create operations

### Requirement: Publish game state updated integration event
The system SHALL publish a `GameStateUpdatedEvent` to RabbitMQ through MassTransit when `GameStateService` successfully persists a game state update, including when game-state updates are orchestrated through `IRequest<TResponse>`-based game-logic handlers, and integration tests SHALL verify publication conditions.

#### Scenario: Game update succeeds and event is published
- **GIVEN** a valid move request and successful state persistence
- **WHEN** `GameStateService` applies and persists a valid game state update
- **THEN** the system publishes exactly one `GameStateUpdatedEvent` containing the game identifier and updated state details

#### Scenario: Game update fails and no event is published
- **GIVEN** a game state update that fails before persistence commit
- **WHEN** the update operation exits with failure
- **THEN** the system MUST NOT publish a `GameStateUpdatedEvent`

#### Scenario: Integration tests validate game updated publication path
- **GIVEN** Testcontainers-based dependencies for GameStateService integration tests
- **WHEN** the move/update runtime path is executed in integration tests
- **THEN** the suite SHALL assert that a `GameStateUpdatedEvent` is published only for successful persisted updates

### Requirement: Configure messaging transport for event publishing
The system SHALL configure MassTransit to use RabbitMQ for publishing `GameCreatedEvent` and `GameStateUpdatedEvent` in each deployed environment.

#### Scenario: Valid broker configuration provided
- **WHEN** the application starts with valid RabbitMQ configuration
- **THEN** MassTransit publisher infrastructure is initialized and ready before `GameStateService` handles requests

#### Scenario: Broker configuration is missing or invalid
- **WHEN** the application starts without valid RabbitMQ configuration for required environments
- **THEN** startup fails with actionable configuration errors

### Requirement: Preserve API compatibility while adding events
The system MUST preserve existing REST API request and response contracts while adding event publication behavior.

#### Scenario: Existing create endpoint contract remains unchanged
- **WHEN** clients call the game create endpoint after this change
- **THEN** request and response payload structures remain backward compatible

#### Scenario: Existing update endpoint contract remains unchanged
- **WHEN** clients call the game update endpoint after this change
- **THEN** request and response payload structures remain backward compatible
