## ADDED Requirements

### Requirement: Consume GameCreated event in GameNotificationService
The system SHALL configure `GameNotificationService` to consume `GameCreated` messages from RabbitMQ using MassTransit.

#### Scenario: GameCreated message is received
- **GIVEN** `GameNotificationService` is running with valid RabbitMQ and MassTransit configuration
- **WHEN** a `GameCreated` message is published to the broker
- **THEN** the `GameCreated` consumer in `GameNotificationService` SHALL receive the message

#### Scenario: GameCreated message handling is bootstrap-only
- **WHEN** the `GameCreated` consumer handles a received message
- **THEN** the consumer SHALL only execute `Console.WriteLine` output and SHALL NOT call persistence or external notification providers

### Requirement: Consume GameStateUpdated event in GameNotificationService
The system SHALL configure `GameNotificationService` to consume `GameStateUpdated` messages from RabbitMQ using MassTransit.

#### Scenario: GameStateUpdated message is received
- **GIVEN** `GameNotificationService` is running with valid RabbitMQ and MassTransit configuration
- **WHEN** a `GameStateUpdated` message is published to the broker
- **THEN** the `GameStateUpdated` consumer in `GameNotificationService` SHALL receive the message

#### Scenario: GameStateUpdated message handling is bootstrap-only
- **WHEN** the `GameStateUpdated` consumer handles a received message
- **THEN** the consumer SHALL only execute `Console.WriteLine` output and SHALL NOT call persistence or external notification providers

### Requirement: Register consumer endpoints at startup
The system SHALL register MassTransit consumers and RabbitMQ receive endpoints during `GameNotificationService` startup.

#### Scenario: Startup succeeds with valid messaging configuration
- **WHEN** the service starts with required RabbitMQ configuration values
- **THEN** MassTransit SHALL initialize and bind receive endpoints for both event consumers before serving traffic

#### Scenario: Startup fails with missing messaging configuration
- **WHEN** required RabbitMQ configuration is missing or invalid
- **THEN** service startup SHALL fail with actionable configuration errors
