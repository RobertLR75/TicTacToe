## ADDED Requirements

### Requirement: Notification service supports event-driven intake
The `GameNotificationService` capability SHALL include event-driven intake by consuming `GameCreated` and `GameStateUpdated` integration events over RabbitMQ via MassTransit.

#### Scenario: Service receives lifecycle events without API contract changes
- **GIVEN** `GameNotificationService` has both REST endpoints and MassTransit consumers configured
- **WHEN** game lifecycle events are published while existing API consumers continue normal requests
- **THEN** the service SHALL consume events and SHALL preserve existing REST API request and response contracts

#### Scenario: Event intake remains non-breaking during bootstrap phase
- **WHEN** an event is consumed by `GameNotificationService`
- **THEN** handling SHALL be limited to `Console.WriteLine` logging and SHALL NOT alter existing endpoint behavior or response payloads
