## ADDED Requirements

### Requirement: GameNotificationService publishes game-created notifications from consumed events
`NotificationGameService` SHALL publish a SignalR `GameCreatedNotification` to the target game audience whenever a `GameCreatedEvent` is consumed.

#### Scenario: GameCreatedEvent produces GameCreatedNotification
- **GIVEN** `NotificationGameService` is subscribed to game lifecycle events
- **WHEN** a valid `GameCreatedEvent` is consumed
- **THEN** the service SHALL map the event payload to `GameCreatedNotification`
- **AND** the service SHALL publish the SignalR notification to clients scoped to the event `gameId`

#### Scenario: Invalid GameCreatedEvent payload is not published
- **GIVEN** `NotificationGameService` receives a `GameCreatedEvent`
- **WHEN** required notification fields are missing or invalid
- **THEN** the service SHALL NOT publish a malformed SignalR notification
- **AND** the service SHALL record an error log with correlation details

### Requirement: GameNotificationService publishes game-state-updated notifications from consumed events
`NotificationGameService` SHALL publish a SignalR `GameStateUpdatedNotification` to the target game audience whenever a `GameStateUpdatedEvent` is consumed.

#### Scenario: GameStateUpdatedEvent produces GameStateUpdatedNotification
- **GIVEN** `NotificationGameService` is subscribed to game lifecycle events
- **WHEN** a valid `GameStateUpdatedEvent` is consumed
- **THEN** the service SHALL map the event payload to `GameStateUpdatedNotification`
- **AND** the service SHALL publish the SignalR notification to clients scoped to the event `gameId`

#### Scenario: Notification ordering is preserved per game
- **GIVEN** two `GameStateUpdatedEvent` messages for the same `gameId` are consumed in sequence
- **WHEN** notifications are published to SignalR
- **THEN** the resulting `GameStateUpdatedNotification` messages SHALL preserve consume order for that `gameId`

### Requirement: GameNotificationService is the sole game SignalR publisher for lifecycle notifications
The system SHALL centralize game lifecycle SignalR publishing (`GameCreatedNotification` and `GameStateUpdatedNotification`) in `GameNotificationService` instead of `GameStateService`.

#### Scenario: No duplicate lifecycle notifications from multiple services
- **GIVEN** a game lifecycle event is consumed
- **WHEN** notifications are emitted
- **THEN** only `GameNotificationService` SHALL publish lifecycle SignalR notifications for that event
- **AND** clients SHALL receive one notification per event type occurrence
