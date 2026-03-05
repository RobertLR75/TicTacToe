## Why

The GameNotificationService needs to react to game lifecycle events from RabbitMQ so it can evolve from a passive component into an event-driven participant. Implementing baseline consumers now provides a safe integration point for future notification workflows while validating MassTransit wiring end to end.

## What Changes

- Add MassTransit consumer classes in GameNotificationService for `GameCreated` and `GameStateUpdated` messages.
- Configure RabbitMQ transport and consumer registration in GameNotificationService startup/composition root.
- Implement temporary handling behavior that only logs receipt using `Console.WriteLine`.
- Add basic observability around message handling path (startup wiring and message receipt logging).
- Document operational rollback steps to disable consumers and return to current behavior.

## Capabilities

### New Capabilities
- `game-notification-event-consumers`: Consume `GameCreated` and `GameStateUpdated` events from RabbitMQ in GameNotificationService using MassTransit.

### Modified Capabilities
- `notification-api`: Notification service behavior extends to event-driven intake by consuming game domain events used to trigger future notifications.

## Impact

- Affected code: GameNotificationService messaging configuration and new consumer classes for game events.
- APIs/contracts: Depends on existing message contracts for `GameCreated` and `GameStateUpdated`; no public REST API changes.
- Dependencies/systems: RabbitMQ and MassTransit configuration become active runtime dependencies for GameNotificationService.
- Performance impact analysis: Adds background consumer workload and message deserialization overhead; expected low impact because handlers only perform `Console.WriteLine` in this phase.
- Affected teams: Backend/API team (event publishing contracts), Notification service owners, and DevOps/platform team (RabbitMQ connectivity/config).
- Rollback plan: Remove/disable consumer registration and RabbitMQ receive endpoints via configuration or revert this change set, then redeploy GameNotificationService.
