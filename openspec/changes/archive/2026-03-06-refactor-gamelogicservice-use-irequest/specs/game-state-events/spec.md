## MODIFIED Requirements

### Requirement: Publish game state updated integration event
The system SHALL publish a `GameStateUpdatedEvent` to RabbitMQ through MassTransit when `GameStateService` successfully persists a game state update, including when game-state updates are orchestrated through `IRequest<TResponse>`-based game-logic handlers.

#### Scenario: Game update succeeds and event is published
- **WHEN** `GameStateService` applies and persists a valid game state update
- **THEN** the system publishes exactly one `GameStateUpdatedEvent` containing the game identifier and updated state details

#### Scenario: Game update fails and no event is published
- **WHEN** a game state update fails before persistence is committed
- **THEN** the system MUST NOT publish a `GameStateUpdatedEvent`
