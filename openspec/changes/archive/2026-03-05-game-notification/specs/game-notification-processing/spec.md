## ADDED Requirements

### Requirement: Process GameCreated events into notifications
The system SHALL consume `GameCreated` events and persist a corresponding notification record.

#### Scenario: GameCreated event is successfully processed
- **WHEN** `GameNotificationService` receives a valid `GameCreated` event
- **THEN** the system SHALL store one notification record with event type, game identifier, and created timestamp

#### Scenario: Duplicate GameCreated event is received
- **WHEN** the same `GameCreated` event is delivered more than once
- **THEN** the system SHALL avoid creating duplicate persisted notifications for the same event identity

### Requirement: Process GameStateUpdated events into notifications
The system SHALL consume `GameStateUpdated` events and persist a corresponding notification record.

#### Scenario: GameStateUpdated event is successfully processed
- **WHEN** `GameNotificationService` receives a valid `GameStateUpdated` event
- **THEN** the system SHALL store one notification record with event type, game identifier, and update timestamp

#### Scenario: Event payload is invalid
- **WHEN** an event required fields are missing or malformed
- **THEN** the system SHALL reject persistence for that event and emit diagnostic error logging

### Requirement: Event processing can be operationally disabled
The system SHALL support disabling event-driven notification processing through configuration.

#### Scenario: Processing toggle is off
- **WHEN** `GameNotificationService` starts with event processing disabled
- **THEN** consumer processing SHALL not persist notifications

#### Scenario: Processing toggle is on
- **WHEN** `GameNotificationService` starts with event processing enabled and valid messaging configuration
- **THEN** consumers SHALL process incoming events according to notification persistence requirements
